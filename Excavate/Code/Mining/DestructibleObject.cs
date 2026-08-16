
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
   [SerializeField] private int ToolLvlRequired = 1;
   [SerializeField] private float hp;
   [SerializeField, Min(0)] private int materialsOnDestroy = 1;
   public void TakeDamage(float damage)
   {
      hp -= damage;
      if (hp <= 0)
      {
         
         ResourceManager.Instance.AddMaterials(materialsOnDestroy);
         Destroy(gameObject);
      }
   }
   public int GetToolLvlRequired()
   {
      return ToolLvlRequired;
   }
   
}

