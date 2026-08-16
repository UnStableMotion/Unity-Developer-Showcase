
using System.Collections.Generic;
using UnityEngine;

public class Chunk //Helper class for managing chunks of objects in the game world
{
   public Vector2Int coordinates;
   public List<ChunkObject> objects;
   

}



public class ChunkManager : MonoBehaviour
{
   [SerializeField,Min(0)] private Vector2Int renderRadius;
   [SerializeField] private int chunkSize;
   [SerializeField] int activatePerFrame;
   [SerializeField] int deactivatePerFrame;
   [SerializeField] private int chunkOffsetZ;
   [SerializeField] private Transform PlayerTransform;
   private Vector2Int currentPlayerChunk;
   public static ChunkManager instance;
   private Dictionary<Vector2Int, Chunk> chunks = new();
   private HashSet<Vector2Int> activeChunks = new();
   private Queue<ChunkObject> ActivateQueue = new();
   private Queue<ChunkObject> DeactivateQueue = new();
   HashSet<ChunkObject> queuedForActivation = new();
   HashSet<ChunkObject> queuedForDeactivation  = new();
   private Dictionary<ChunkObject, bool> targetState = new();

   private void Awake()
   {
      instance = this;
   }
   void Start()
   {
      ChunkObject[] chunkObjects =
    Object.FindObjectsByType<ChunkObject>(FindObjectsInactive.Include);//Find all ChunkObject components, including inactive ones

      foreach (ChunkObject chunkObject in chunkObjects)//Go through every found object, register it in a chunk, and deactivate it
      {
         RegisterObjectInChunk(chunkObject);
         chunkObject.gameObject.SetActive(false);
      }

      currentPlayerChunk = GetPlayerChunkCoordinates();//Then get the chunk the player is in and activate all chunks around it
      UpdateActiveChunks();
 
   }



   public void Update()
   {
      Vector2Int newPlayerChunk = GetPlayerChunkCoordinates();
      
      if (newPlayerChunk != currentPlayerChunk)
      {
         currentPlayerChunk = newPlayerChunk;
         UpdateActiveChunks();
      }

      //Activation queue
      for(int i=0; i < activatePerFrame; i++)
      {
         if (ActivateQueue.Count == 0)
            break;//Extra check for when the queue runs out

         ChunkObject obj = ActivateQueue.Dequeue();

         //The object is physically no longer in ActivateQueue
         queuedForActivation.Remove(obj);

         if (!targetState.TryGetValue(obj, out bool state))//Check whether we still have a target state for this object
            continue;
         if (!state)//If the target state is false, skip activation
            continue;
         

         if (!obj.gameObject.activeSelf)//If the object is not already active
         {
            obj.gameObject.SetActive(true);
         }
         targetState.Remove(obj);


      }

      //Deactivation queue
      for (int i = 0; i < deactivatePerFrame; i++)//Same process as activation
      {
         if (DeactivateQueue.Count == 0)
            break;

         ChunkObject obj = DeactivateQueue.Dequeue();

         queuedForDeactivation.Remove(obj);

         if (!targetState.TryGetValue(obj, out bool state))
            continue;

         if (state) //If the target state is true, skip deactivation
            continue;


         if (obj.gameObject.activeSelf)
         {
            obj.gameObject.SetActive(false);
         }

         targetState.Remove(obj);
      }

   }
   private Vector2Int GetPlayerChunkCoordinates()
   {
      return new Vector2Int(Mathf.FloorToInt(PlayerTransform.position.x / chunkSize), Mathf.FloorToInt(PlayerTransform.position.z / chunkSize));  //Calculate the player's chunk from their position and the chunk size

   }
   private void UpdateActiveChunks()
   {
      HashSet<Vector2Int> requiredChunks = GetRequiredChunks();
      
      foreach (Vector2Int coordinates in activeChunks)
      {
         if (!requiredChunks.Contains(coordinates))//If a chunk from the previous scan is not in the new scan, deactivate it
         {
            // Deactivate the chunk
            DeactivateChunk(coordinates);
         }
      }
      foreach(Vector2Int coordinates in requiredChunks)
      {
         if(!activeChunks.Contains(coordinates))//If the new scan has a chunk that was not active before, activate it
         {
            // Activate the chunk
            ActivateChunk(coordinates);
         }
      }
      activeChunks = requiredChunks;
   }
   private HashSet<Vector2Int> GetRequiredChunks()
   {
      HashSet<Vector2Int> ChunkToActivate = new();
      for (int x = -renderRadius.x; x <= renderRadius.x; x++)
      {
         for (int z = -renderRadius.y+chunkOffsetZ; z <= renderRadius.y+chunkOffsetZ; z++)
         {
            Vector2Int chunkToLoad =
              currentPlayerChunk + new Vector2Int(x, z);//Calculate the chunk to load from the player's chunk and the render radius
            ChunkToActivate.Add(chunkToLoad);//Add the chunk coordinates to the set of active chunks
         }
      }
     return ChunkToActivate;
   }
   private void DeactivateChunk(Vector2Int coordinates)
   {
      if (chunks.TryGetValue(coordinates, out Chunk chunk))//If we successfully get the chunk at these coordinates
      {
         foreach (ChunkObject chunkobject in chunk.objects)
         {
            targetState[chunkobject] = false;

            if (queuedForDeactivation.Add(chunkobject))//Returns true if the object was added, not replaced
            {
               DeactivateQueue.Enqueue(chunkobject);
            }
         }
      }
   }
   private void ActivateChunk(Vector2Int coordinates)
   {
      if (chunks.TryGetValue(coordinates, out Chunk chunk))
      {
         foreach (ChunkObject chunkobject in chunk.objects)
         {
            targetState[chunkobject] = true;//Mark the object as "needs activation"
            if (queuedForActivation.Add(chunkobject))//Add returns true if the object was not already in the HashSet
            {
               ActivateQueue.Enqueue(chunkobject);
            }
         }
      }
   }
   public void RegisterObjectInChunk(ChunkObject chunkObject)
   {
      Vector2Int chunkCoordinates = new(Mathf.FloorToInt(chunkObject.transform.position.x / chunkSize), Mathf.FloorToInt(chunkObject.transform.position.z / chunkSize));//Calculate the chunk coordinates from the object's position and the chunk size
      if (!chunks.TryGetValue(chunkCoordinates, out Chunk chunk))//If the chunk does not exist yet, create it
      {
         chunk = new()//Short Chunk creation: the type comes from the left side
         {
            coordinates = chunkCoordinates,//Set the chunk coordinates
            objects = new List<ChunkObject>()//Initialize the list of objects in the chunk
         };//Create a new chunk
         chunks.Add(chunkCoordinates, chunk);//Add the chunk to the dictionary
         
      }
      chunk.objects.Add(chunkObject);//Add the object to the chunk's list
     
   }
   public void UnregisterObjectFromChunk(ChunkObject objectToUnregister)//Remove the object, then remove the chunk too if it has no objects left
   {
      Vector2Int chunkCoordinates = new(Mathf.FloorToInt(objectToUnregister.transform.position.x / chunkSize), Mathf.FloorToInt(objectToUnregister.transform.position.z / chunkSize));//Calculate the chunk coordinates from the object's position and the chunk size
      if (chunks.TryGetValue(chunkCoordinates, out Chunk chunk))//If the chunk exists, remove the object from it
      {
         chunk.objects.Remove(objectToUnregister);//Remove the object from the chunk's list
         if (chunk.objects.Count == 0)//If the chunk has no objects left, remove it from the dictionary
         {
            chunks.Remove(chunkCoordinates);
         }
      }
   }
}
