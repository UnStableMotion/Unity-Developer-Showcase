using UnityEngine;

public class ToolDamage : MonoBehaviour
{
   [SerializeField] private float ToolLvl = 1;

private void OnTriggerEnter(Collider other)
   {
      DestructibleObject destructible = other.GetComponentInParent<DestructibleObject>();
      
      if (destructible != null)
      {
         int toolLevelRequired = destructible.GetToolLvlRequired();
         if (ToolLvl >= toolLevelRequired)
         {
            destructible.TakeDamage(ToolLvl);
         }
         else
         {
            UIManager.instance.WeekToolAnimationShow();
         }
      }
   }

   public void SetToolLvl(float newToolLvl)
   {
      ToolLvl = newToolLvl;
   }


}

