using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Event", menuName = "Event")]
public class Event : ScriptableObject
{
    UnityEvent e;
    public void Subscribe(UnityAction action)
    {
        e.AddListener(action);
    }
    public void Invoke()
    {
        e.Invoke();
    }

    public void Unsubscribe(UnityAction action)
    {
        e.RemoveListener(action);
    }

}
