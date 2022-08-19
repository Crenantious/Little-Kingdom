using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [field: SerializeField] public UnitType UnitType { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
}
