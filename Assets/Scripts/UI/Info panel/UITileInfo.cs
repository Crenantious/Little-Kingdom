using UnityEngine;
using TMPro;

public class UITileInfo : UIInfoPanel
{
    [SerializeField] TMP_Text info;

    public override void DisplayInfo(Component component) =>
        info.text = $"{((Tile)component).Resource.name} tile";
}
