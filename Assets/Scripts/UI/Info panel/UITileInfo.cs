using UnityEngine;
using TMPro;

public class UITileInfo : UIInfoPanel
{
    [SerializeField] TMP_Text info;
    private void Start() =>
        Register<Tile>(this);

    protected override void DisplayInfo(Component component) =>
        info.text = $"{((Tile)component).Resource.name} tile";
}
