using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventInfo
{
    public GameObject gameObject;

    public EventInfo(GameObject gameObject = default)
    {
        this.gameObject = gameObject;
    }
}
