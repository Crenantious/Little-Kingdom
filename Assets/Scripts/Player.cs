using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Zenject;

public class Player
{
    public Town town;

    [Inject] readonly Factories.Town townFactory;
    [Inject] readonly UIPlayerInfoPanels uiPlayerInfoPanels;
    readonly Dictionary<Resource, int> resources = new();

    [Inject]
    public void Constrct()
    {
        town = townFactory.Create(this);
        foreach (var resource in References.resources)
            resources.Add(resource, 0);
    }

    public int GetResourceAmount(Resource resource) =>
        resources[resource];

    public void AddResource(Resource resource, int amount)
    {
        resources[resource] += amount;
        if (uiPlayerInfoPanels)
            uiPlayerInfoPanels.SetResourceAmount(this, resource, resources[resource]);
    }
}
