using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIResourceAndValues : MonoBehaviour
{
    readonly Dictionary<Resource, UIResourceAndValue> resourceContainers = new();
    readonly List<UIResourceAndValue> resourceAndValues = new();

    [SerializeField] List<Resource> resourceDisplayOrder;
    [SerializeField] UIResourceAndValue resourceContainer;
    [SerializeField] TMP_Text playerName;

    public void Initialise()
    {
        foreach (Resource resource in resourceDisplayOrder)
        {
            var rc = Instantiate(resourceContainer, resourceContainer.transform.parent);
            rc.Initialise(resource, 0);
            resourceContainers.Add(resource, rc);
            resourceAndValues.Add(rc);
        }
    }

    public void SetResourceValue(Resource resource, int amount)
    {
        if(resourceContainers.TryGetValue(resource, out UIResourceAndValue rav))
            rav.SetValue(amount);
    }

    public List<UIResourceAndValue> GetResourceAndValues() =>
        resourceAndValues;
}
