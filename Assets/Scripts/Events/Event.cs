using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Event", menuName = "Event/Event")]
public class Event : ScriptableObject
{
    UnityEvent e;
    public EventInfo eventInfo;

    public void Subscribe(UnityAction action)
    {
        e.AddListener(action);
    }
    public void Invoke(EventInfo eventInfo = null)
    {
        this.eventInfo = eventInfo;
        e.Invoke();
    }
    public void Invoke(EventInfoMono eventInfoMono = null)
    {
        eventInfo = eventInfoMono.eventInfo;
        e.Invoke();
    }
    public void Invoke(EventInfoSO eventInfoSO = null)
    {
        eventInfo = eventInfoSO.eventInfo;
        e.Invoke();
    }

    public void Unsubscribe(UnityAction action)
    {
        e.RemoveListener(action);
    }

}
