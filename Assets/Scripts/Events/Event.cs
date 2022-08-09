using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Event", menuName = "Event/Event")]
public class Event : ScriptableObject
{
    List<UnityAction> actions = new();
    public EventInfo eventInfo;
    public EventConstraints eventConstraints;

    public void Subscribe(UnityAction action)
    {
        actions.Add(action);
    }

    public void Invoke(EventInfo eventInfo = null)
    {
        this.eventInfo = eventInfo;
        Invoke();
    }

    public void Invoke(EventInfoMono eventInfoMono = null)
    {
        eventInfo = eventInfoMono.eventInfo;
        Invoke();
    }

    public void Invoke(EventInfoSO eventInfoSO = null)
    {
        eventInfo = eventInfoSO.eventInfo;
        Invoke();
    }

    public void Unsubscribe(UnityAction action)
    {
        if (actions.Contains(action))
        {
            actions.Remove(action);
        }
    }


    void Invoke()
    {
        foreach (UnityAction action in actions)
        {

        }
    }
    bool CheckConstraints(UnityAction action, EventConstraints eventConstraints)
    {
        bool invokeable = true;
        return invokeable;
    }

    bool CheckGameObjectConstraint(UnityAction action, EventConstraints.GameObjectConstraint constraint)
    {
        //bool satisfied = true;
        //(constraint.constraint) switch
        //{
        //    EventConstraints.GameObjectConstraintType.InstanceOfPrefab => PrefabUtility.GetPrefabInstanceStatus(),
        //}
        return false;
    }
}
