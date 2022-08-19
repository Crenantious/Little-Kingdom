using UnityEngine;
using TMPro;

public class UIUnitInfo : UIInfoPanel
{
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text description;

    void Start() =>
        Register<Unit>(this);

    protected override void DisplayInfo(Component component)
    {
        title.text = ((Unit)component).UnitType.name;
        description.text = ((Unit)component).Description;
    }
}
