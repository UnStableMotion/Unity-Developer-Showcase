using System.Collections;
using UnityEngine;

public class AttackPivot : MonoBehaviour
{
   [SerializeField] private GameObject attackingSphere;
   [SerializeField] private Transform attackPivot;
   [SerializeField] private float MiningAngle;
   [SerializeField] private float CurrentSwingDuration;
   [SerializeField] private float CurrentSwingCooldown;
   [SerializeField] private TrailRenderer trail;
   [SerializeField] private float BaseSwingDuration;
   [SerializeField] private float BaseSwingCooldown;


   void Start()
   {

     StartCoroutine(AttackLoop());
   }
   private IEnumerator AttackLoop()
   {
      while (true)
      {
         yield return AttackCoroutine();
         yield return new WaitForSeconds(CurrentSwingCooldown);
      }
   }

   private IEnumerator AttackCoroutine()
   {
      float attackDirectionY = transform.eulerAngles.y;//Сохраняем начальное направление взгляда игрока по оси Y, чтобы использовать его для вращения атаки.
      Quaternion attackDirection = Quaternion.Euler(0f, attackDirectionY, 0f);//Создаем кватернион для направления атаки на основе сохраненного угла поворота по оси Y.

      attackingSphere.SetActive(true);
      trail.emitting = true;


      float elapsedTime = 0f;
      float halfAngle = MiningAngle / 2f;
      float baseAngle = 90f;
      float startAngle = baseAngle - halfAngle;
      float endAngle = baseAngle + halfAngle;

      while (elapsedTime < CurrentSwingDuration)
      {
         elapsedTime += Time.deltaTime;
         float progress = elapsedTime / CurrentSwingDuration;//Progress of the attack animation from 0 to 1.
         float currentRotation = Mathf.Lerp(startAngle, endAngle, progress);//Rotate smoothly from the start angle to the end angle.
         Quaternion swingRotation = Quaternion.Euler(0f, currentRotation, 0f);//Current local swing rotation.
         Quaternion finalRotation = attackDirection * swingRotation;//Apply the swing relative to the player's facing direction.
         attackPivot.rotation = finalRotation;//Rotate the attack pivot.
         yield return null;
      }

      attackPivot.rotation = attackDirection * Quaternion.Euler(0f, endAngle, 0f);//Snap exactly to the final attack angle to avoid precision errors.


      attackingSphere.SetActive(false);
      trail.emitting = false;
      trail.Clear();
   }
   public void MiningAngleChange(int amount)
   {
      MiningAngle = amount;
   }
   public float GetBaseSwingTimeDuration()
   {
      return BaseSwingDuration;
   }
      public void SwingTimeChange(float amount)
   {
      CurrentSwingDuration = amount;
   }
   public float GetBaseCooldown()
   {
      return BaseSwingCooldown;
   }
   public void CooldownChange(float amount)
   {
      CurrentSwingCooldown = amount;
   }

}
