using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;

public class UIPlayerInfoPanel : MonoBehaviour
{
    public Player player;

    [SerializeField] UIResourceAndValues uiResourceAndValues;

    public void Initialise(Player player)
    {
        this.player = player;
        uiResourceAndValues.Initialise();
    }

    public void SetResourceAmount(Resource resource, int amount) =>
        uiResourceAndValues.SetResourceValue(resource, amount);
}
