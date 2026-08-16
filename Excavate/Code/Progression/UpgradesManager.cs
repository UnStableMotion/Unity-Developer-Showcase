using TMPro;
using UnityEngine;

[System.Serializable]
public class UpgradeLevel
{
   public int price;
   public float value;
}



[System.Serializable]
public class UpgradesData
{
   [SerializeField] public UpgradeLevel[] levels;
   public int currentLevel;
   [SerializeField] public TextMeshProUGUI NextStatValueText;
   [SerializeField] public TextMeshProUGUI PriceText;
   [SerializeField] public TextMeshProUGUI CurrentLvl;

}
public class UpgradesManager : MonoBehaviour
{
   [SerializeField] private UpgradesData SwingSpeedUpgrade;
   [SerializeField] private UpgradesData radiusUpgrade;
   [SerializeField] private UpgradesData hardnessUpgrade;
   [SerializeField] private AttackPivot attackPivot;
   [SerializeField] private ToolDamage Hardness;
   [SerializeField] private GameObject[] ToolPrefabs;
   
   private void Start()
   {
      UpdateUpgradeText(SwingSpeedUpgrade, "x");
      UpdateUpgradeText(radiusUpgrade, "°");
      UpdateUpgradeText(hardnessUpgrade);
   }
   public void UpgradeHardness()
   {
      if (TryBuyUpgrade(hardnessUpgrade))
      {
         Hardness.SetToolLvl(hardnessUpgrade.levels[hardnessUpgrade.currentLevel].value);
         if (ToolPrefabs != null && hardnessUpgrade.currentLevel < ToolPrefabs.Length)
         {
            ToolPrefabs[hardnessUpgrade.currentLevel - 1].SetActive(false);
            ToolPrefabs[hardnessUpgrade.currentLevel].SetActive(true);
         }
         UpdateUpgradeText(hardnessUpgrade);
      }
   }
   public void UpgradeSwingSpeed()
   {
      if (TryBuyUpgrade(SwingSpeedUpgrade))
      {
         attackPivot.CooldownChange(attackPivot.GetBaseCooldown()/SwingSpeedUpgrade.levels[SwingSpeedUpgrade.currentLevel].value);//cooldown = currentcooldown / upgrade value, because the upgrade value is a multiplier for the swing speed
         attackPivot.SwingTimeChange(attackPivot.GetBaseSwingTimeDuration()/SwingSpeedUpgrade.levels[SwingSpeedUpgrade.currentLevel].value);//swingtime = currentswingtime / upgrade value, because the upgrade value is a multiplier for the swing speed
         UpdateUpgradeText(SwingSpeedUpgrade);
      }
   }
   public void UpgradeRadius()
   {
      if (TryBuyUpgrade(radiusUpgrade))
      {
         attackPivot.MiningAngleChange((int)radiusUpgrade.levels[radiusUpgrade.currentLevel].value);
         UpdateUpgradeText(radiusUpgrade, "°");
      }
   }
   private void UpdateUpgradeText(UpgradesData upgrade, string valueSuffix = "")
   {
      int nextLevel = upgrade.currentLevel + 1;

      upgrade.CurrentLvl.text = "LEVEL " + upgrade.currentLevel;//changes current lvl

      if (nextLevel >= upgrade.levels.Length)// Shows "MAX" if reached max lvl
      {
         upgrade.NextStatValueText.text = "MAX";
         upgrade.PriceText.gameObject.SetActive(false);
         return;
      }

      upgrade.PriceText.gameObject.SetActive(true);
      upgrade.PriceText.text = upgrade.levels[nextLevel].price.ToString();
      upgrade.NextStatValueText.text = string.Format(
         "{0}{2}   -→   {1}{2}",
         upgrade.levels[upgrade.currentLevel].value,
         upgrade.levels[nextLevel].value,
         valueSuffix);//using suffix to add a unit to the value, like ° for radius or m/s for speed
   }
   private bool TryBuyUpgrade(UpgradesData upgrade)
   {
      int nextLevel = upgrade.currentLevel + 1;
      if (upgrade.levels == null || nextLevel >= upgrade.levels.Length)
      {
         Debug.Log("Max level reached for " + upgrade.GetType().Name);
         return false;
      }

      int price = upgrade.levels[nextLevel].price;
      if (ResourceManager.Instance.TrySpendMaterials(price))// Check if the player has enough materials to buy the upgrade
      {
         upgrade.currentLevel++;
         return true;
      }
      else
      {
         Debug.Log("Not enough materials to upgrade " + upgrade.GetType().Name);
         return false;
         //Дописать логику появления окошка с информацией о нехватке ресурсов
      }
   }
  
}

