using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Event", menuName = "Event/Event")]
public class Event : ScriptableObject
{
    List<(UnityAction, EventConstraints)> actionsAndConstraints = new();
    public EventInfo eventInfo;
    public EventConstraints eventConstraints;

    public void Subscribe(UnityAction action, EventConstraints eventConstraints = null)
    {
        actionsAndConstraints.Add((action, eventConstraints));
    }

    public void Invoke(EventInfo eventInfo = null)
    {
        this.eventInfo = eventInfo;
        InvokeSubscribers();
    }

    public void Invoke(EventInfoMono eventInfoMono = null)
    {
        eventInfo = eventInfoMono.eventInfo;
        InvokeSubscribers();
    }

    public void Invoke(EventInfoSO eventInfoSO = null)
    {
        eventInfo = eventInfoSO.eventInfo;
        InvokeSubscribers();
    }

    public void Invoke()
    {
        eventInfo = null;
        InvokeSubscribers();
    }

    void InvokeSubscribers()
    {
        foreach ((UnityAction, EventConstraints) actionAndConstraint in actionsAndConstraints)
        {
            if (CheckConstraints(actionAndConstraint))
                actionAndConstraint.Item1.Invoke();
        }
    }

    public void Unsubscribe(UnityAction action)
    {
        List<UnityAction> onlyActions = (List<UnityAction>)actionsAndConstraints.Select(x => x.Item1);
        int index = onlyActions.IndexOf(action);
        if (index != -1)
        {
            actionsAndConstraints.RemoveAt(index);
        }
    }


    bool CheckConstraints((UnityAction, EventConstraints) actionAndConstraint)
    {
        if (actionAndConstraint.Item2 == null) return true;

        if (!CheckGameObjectConstraints(actionAndConstraint.Item2.gameObjectConstraints) ||
            !CheckGameStateConstraint())
            return false;

        return true;
    }

    bool CheckGameStateConstraint()
    {
        return true;
    }

    bool CheckGameObjectConstraints(List<EventConstraints.GameObjectConstraint> constraints)
    {
        if (eventInfo == null || eventInfo.gameObject == null || constraints.Count == 0) return false;

        bool isSatisfied = true;
        foreach (EventConstraints.GameObjectConstraint constraint in constraints)
        {
            if (constraint.gameObject == null) return false;

            isSatisfied = constraint.constraint switch
            {
                EventConstraints.GameObjectConstraintType.InstanceOfPrefab =>
                    constraint.gameObject.TryGetComponent(out PrefabReference prefabReference) &&
                    eventInfo.gameObject.TryGetComponent(out PrefabReference prefabReference1) &&
                    prefabReference.GUID == prefabReference1.GUID,
                _ => true
            };

            if (!isSatisfied) return false;
        }
        return isSatisfied;
    }
}
