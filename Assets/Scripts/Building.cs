using CollectionsExtensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        public List<ResourceCost> resourceCosts;
        
        public bool TryGetResourceCost(Resource resource, out int cost)
        {
            if (resourceCosts != null)
            {
                foreach (ResourceCost resourceCost in resourceCosts)
                {
                    if (resourceCost.resource == resource)
                    {
                        cost = resourceCost.cost;
                        return true;
                    }
                }
            }
            cost = default;
            return false;
        }
    }

    [System.Serializable]
    public struct ResourceCost
    {
        public Resource resource;
        public int cost;
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

        foreach (ResourceCost cost in UpgradeCosts[Tier].resourceCosts)
        {
            if (Town.player.GetResourceAmount(cost.resource) < cost.cost)
                return false;
        }
        return true;
    }

    void DeductPlayerResourcesForUpgrade()
    {
        foreach (ResourceCost cost in UpgradeCosts[Tier].resourceCosts)
            Town.player.AddResource(cost.resource, -cost.cost);
    }
}
