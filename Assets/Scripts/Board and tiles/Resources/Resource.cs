using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile type")]
public class Resource : ScriptableObject
{
    public Material material;

    private void OnEnable() =>
        References.resources.Add(this);
}
