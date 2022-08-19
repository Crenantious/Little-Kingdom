using System.Collections.Generic;
using UnityEngine;

public class UIPlayerInfoPanels : MonoBehaviour
{
    readonly Dictionary<Player, UIPlayerInfoPanel> panels = new();

    [SerializeField] UIPlayerInfoPanel panel;

    public void AddPanel(Player player)
    {
        var p = Instantiate(panel, transform);
        p.gameObject.SetActive(true);
        Utilities.ResetTransformLocally(p.transform);
        p.Initialise(player);
        panels.Add(player, p);
    }

    public void SetResourceAmount(Player player, Resource resource, int amount) =>
        panels[player].SetResourceAmount(resource, amount);
}
