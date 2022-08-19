using CollectionsExtensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DataContainers;

public class Building : MonoBehaviour
{
    [field: SerializeField] public BuildingType BuildingType { get; private set; }
    [field: SerializeField] public Town Town { get; private set; }
    [field: SerializeField] public int Tier { get; private set; } = 0;
    [field: SerializeField] public List<UpgradeCost> UpgradeCosts { get; private set; } = new();
    [field: SerializeField] public Description Description { get; private set; }

    [System.Serializable]
    public struct UpgradeCost
    {
        public List<ResourceValue> resourceCosts;
        
        public bool TryGetResourceCost(Resource resource, out int cost)
        {
            if (resourceCosts != null)
            {
                foreach (ResourceValue resourceCost in resourceCosts)
                {
                    if (resourceCost.resource == resource)
                    {
                        cost = resourceCost.value;
                        return true;
                    }
                }
            }
            cost = default;
            return false;
        }
    }

    public UpgradeCost GetUpgradeCost(int tier) =>
        UpgradeCosts.ElementAtOrDefault(tier);

    public virtual void Upgrade()
    {
        if (!CanUpgrade())
            return;

        DeductPlayerResourcesForUpgrade();
        Tier++;
    }

    public bool CanUpgrade()
    {
        if (!UpgradeCosts.TryGetElement(Tier, out _))
            return false;

        foreach (ResourceValue cost in UpgradeCosts[Tier].resourceCosts)
        {
            if (Town.player.GetResourceAmount(cost.resource) < cost.value)
                return false;
        }
        return true;
    }

    void DeductPlayerResourcesForUpgrade()
    {
        foreach (ResourceValue cost in UpgradeCosts[Tier].resourceCosts)
            Town.player.AddResource(cost.resource, -cost.value);
    }
}
