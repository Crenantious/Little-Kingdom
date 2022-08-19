using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Description", menuName = "Game/Description")]
public class Description : ScriptableObject
{
    public List<string> descriptions = new();
}
