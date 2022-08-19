using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInfoPanel : MonoBehaviour
{
    static readonly List<UIInfoPanel> registered = new();

    [field: SerializeField] public GameObject Container { get; protected set; }

    protected void Register<T>(UIInfoPanel derived) where T : Component
    {
        EventConstraints constraints = new();
        constraints.gameObjectConstraints.Add(new(EventConstraints.GameObjectConstraintType.HasComponent,
                                                  componentType: typeof(T)));
        References.GameObjectSelectedEvent.Subscribe(() => DisplayDerivedInfo<T>(derived), constraints);

        registered.Add(derived);
    }

    protected virtual void DisplayInfo(Component component) { }

    void DisplayDerivedInfo<T>(UIInfoPanel derived) where T : Component
    {
        foreach(var panel in registered)
        {
            if (panel == derived)
            {
                Component component = References.GameObjectSelectedEvent.eventInfo.gameObject.GetComponent<T>();
                derived.DisplayInfo(component);
            }
            panel.Container.SetActive(panel == derived);
        }
    }
}
