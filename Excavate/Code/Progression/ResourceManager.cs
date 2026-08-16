using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{

   public static ResourceManager Instance;
   private int materialsBalance;
   [SerializeField] private TextMeshProUGUI materialBalanceCounter;
   private void Awake()//this method is called when the script instance is being loaded. It ensures that there is only one instance of ResourceManager and that it persists across scene loads.
   {
      Instance = this;
     
   }
   public void AddMaterials(int amount)
   {
      materialsBalance += amount;
      UpdateMaterialCounter();
   }
  private void UpdateMaterialCounter()
   {
      materialBalanceCounter.text = materialsBalance.ToString();
   }
   public int GetMaterialsAmount()
   {
      return materialsBalance;
   }
   public bool TrySpendMaterials(int amount)
   {
      if (materialsBalance >= amount)
      {
         materialsBalance -= amount;
         UpdateMaterialCounter();
         return true;
      }
      return false;
   }

}
