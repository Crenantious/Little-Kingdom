using UnityEngine;

public class UIInfoPanelManager : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;
    [SerializeField] UIBuildingInfo buildingInfo;
    [SerializeField] UIUnitInfo unitInfo;
    [SerializeField] UITileInfo tileInfo;
    UIInfoPanel activePanel;
    [field: SerializeField] public GameObject Container { get; protected set; }

    private void Start()
    {
        Register<Building>(buildingInfo);
        Register<Unit>(unitInfo);
        Register<Tile>(tileInfo);
        buildingInfo.gameObject.SetActive(false);
        unitInfo.gameObject.SetActive(false);
        tileInfo.gameObject.SetActive(false);
        mainPanel.SetActive(false);
    }

    protected void Register<T>(UIInfoPanel panel) where T : Component
    {
        EventConstraints constraints = new();
        constraints.gameObjectConstraints.Add(new(EventConstraints.GameObjectConstraintType.HasComponent,
                                                  componentType: typeof(T)));
        References.GameObjectSelectedEvent.Subscribe(() => DisplayInfo<T>(panel), constraints);

        // Inefficient. Should flag if a registered type has been deslected. Then, on the next frame,
        // if no registered type has been selected disable the main panel. Currently this causes
        // unessassary rebuilding of the UI heirarchy if a registered type has been selected when a
        // registered type (different object) was already selected.
        References.GameObjectDeselectedEvent.Subscribe(() => mainPanel.SetActive(false), constraints);
    }

    void DisplayInfo<T>(UIInfoPanel panel) where T : Component
    {
        if (activePanel)
            activePanel.gameObject.SetActive(false);
        activePanel = panel;
        activePanel.gameObject.SetActive(true);
        mainPanel.SetActive(true);

        Component component = References.GameObjectSelectedEvent.eventInfo.gameObject.GetComponent<T>();
        activePanel.DisplayInfo(component);
    }
}
