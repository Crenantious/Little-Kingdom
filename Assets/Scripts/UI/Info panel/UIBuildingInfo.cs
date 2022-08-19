using UnityEngine;
using TMPro;
using CollectionsExtensions;
using UnityEngine.UI;

public class UIBuildingInfo : UIInfoPanel
{
    [SerializeField] Button upgradeButton;
    [SerializeField] TMP_Text currentTierBenefits;
    [SerializeField] TMP_Text nextTierBenefits;
    [SerializeField] UIResourceAndValues uiResourceAndValues;
    Building selectedBuilding;

    void Awake() =>
        uiResourceAndValues.Initialise();

    [System.Serializable]
    public struct ResourceText
    {
        public Resource resource;
        public TMP_Text text;
    }

    public void UpgradeBuilding()
    {
        selectedBuilding.Upgrade();
        DisplayInfo(selectedBuilding);
    }

    public override void DisplayInfo(Component component)
    {
        selectedBuilding = (Building)component;

        if (selectedBuilding.Description)
        {
            if (selectedBuilding.Description.descriptions.TryGetElement(selectedBuilding.Tier, out string description))
                currentTierBenefits.text = description;

            if (selectedBuilding.Description.descriptions.TryGetElement(selectedBuilding.Tier + 1, out description))
                nextTierBenefits.text = description;
        }

        upgradeButton.interactable = selectedBuilding.CanUpgrade();

        var upgradeCost = selectedBuilding.GetUpgradeCost(selectedBuilding.Tier);
        if (upgradeCost.resourceCosts == null)
            throw new System.NullReferenceException($"No upgrade costs defined for {selectedBuilding} at tier {selectedBuilding.Tier}.");

        var containers = uiResourceAndValues.GetResourceAndValues();
        for (int i = 0; i < containers.Count; i++)
        {
            upgradeCost.TryGetResourceCost(containers[i].Resource, out int cost);
            uiResourceAndValues.SetResourceValue(containers[i].Resource, cost);
        }
    }
}
