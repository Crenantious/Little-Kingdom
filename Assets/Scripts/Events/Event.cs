using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Event", menuName = "Events")]
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
