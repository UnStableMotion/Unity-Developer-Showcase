using UnityEngine;

public class ChunkObject : MonoBehaviour
{
   
  
   public void OnDestroy()
   {
      if (ChunkManager.instance != null)
         ChunkManager.instance.UnregisterObjectFromChunk(this);
   }

}
