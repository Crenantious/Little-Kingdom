using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static EventConstraints;

[CreateAssetMenu(fileName = "Event", menuName = "Event/Event")]
public class Event : ScriptableObject
{
    public EventInfo eventInfo;
    public EventConstraints eventConstraints;

    List<(UnityAction, EventConstraints)> actionsAndConstraints = new();
    (UnityAction, EventConstraints) actionAndConstraint;

    public void OnAfterDeserialize()
    {
        eventInfo = null;
        eventConstraints = null;
        actionsAndConstraints = new();
        actionAndConstraint = default;
    }

    /// <summary>
    /// Subscribe to the <see cref="Event"/>. Subscribers will be called when the event is invoked if their constraints are valid.
    /// </summary>
    /// <param name="action">The action to be called.</param>
    /// <param name="eventConstraints">
    /// This is compared to the <see cref="EventInfo"/> given when the <see cref="Event"/> is invoked to see if the action should be called.<br/>
    /// If null, the action is guaranteed to be called.
    /// </param>
    /// <param name="uniqueAction">If true and the action is already subscribed, it will not be subscribed again.</param>
    /// <param name="uniqueActionOverride">
    /// If true this subscription will override all previous subscriptions of the action.<br/>
    /// If false this subscription will be ignored if the action is already subscribed.<br/>
    /// If uniqueAction is false, this parameter is ignored.
    /// </param>
    public void Subscribe(UnityAction action, EventConstraints eventConstraints = null, 
                          bool uniqueAction = true, bool uniqueActionOverride = true)
    {
        if (uniqueAction)
        {
            List<UnityAction> actions = actionsAndConstraints.Select(x => x.Item1).ToList();
            if (actions.Contains(action))
            {
                if (uniqueActionOverride)
                {
                    actionsAndConstraints.RemoveAll(x => x.Item1 == action);
                    actionsAndConstraints.Add((action, eventConstraints));
                }
                return;
            }
        }
        actionsAndConstraints.Add((action, eventConstraints));
    }

    public void Invoke(EventInfo eventInfo = null)
    {
        this.eventInfo = eventInfo;
        TryCallSubscribers();
    }

    public void Invoke(EventInfoMono eventInfoMono = null)
    {
        eventInfo = eventInfoMono.eventInfo;
        TryCallSubscribers();
    }

    public void Invoke(EventInfoSO eventInfoSO = null)
    {
        eventInfo = eventInfoSO.eventInfo;
        TryCallSubscribers();
    }

    public void Invoke()
    {
        eventInfo = null;
        TryCallSubscribers();
    }

    /// <summary>
    /// The action will no longer be called when the <see cref="Event"/> is invoked.<br/>
    /// This will remove all subscriptions of the action.
    /// </summary>
    public void Unsubscribe(UnityAction action) =>
        actionsAndConstraints.RemoveAll(x => x.Item1 == action);

    /// <summary>
    /// Calls each subscribed action if it's given constraints are valid.
    /// </summary>
    void TryCallSubscribers()
    {
        foreach ((UnityAction, EventConstraints) pair in actionsAndConstraints)
        {
            actionAndConstraint = pair;
            if (CheckConstraints())
                actionAndConstraint.Item1.Invoke();
        }
    }

    bool CheckConstraints()
    {
        if (actionAndConstraint.Item2 == null)
            return true;

        if (!CheckGameObjectConstraints(actionAndConstraint.Item2.gameObjectConstraints) ||
            !CheckGameStateConstraint())
            return false;
        return true;
    }

    bool CheckGameStateConstraint() =>
        actionAndConstraint.Item2.states.Contains(References.gameStateManager.CurrentSate) ==
        actionAndConstraint.Item2.allowStates;

    bool CheckGameObjectConstraints(List<GameObjectConstraint> constraints)
    {
        if (constraints.Count == 0)
            return true;

        if (eventInfo == null || !eventInfo.gameObject)
            return false;

        bool isSatisfied = true;
        foreach (var constraint in constraints)
        {
            GameObject gameObject = constraint.gameObject;

            isSatisfied = constraint.constraint switch
            {
                GameObjectConstraintType.InstanceOfPrefab =>
                    gameObject.TryGetComponent(out PrefabReference prefabReference) &&
                    eventInfo.gameObject.TryGetComponent(out PrefabReference prefabReference1) &&
                    prefabReference.GUID == prefabReference1.GUID,
                GameObjectConstraintType.HasComponent =>
                    eventInfo.gameObject.TryGetComponent(constraint.componentType, out Component component),
                _ => throw new NotImplementedException(),
            };

            if (!isSatisfied)
                return false;
        }
        return isSatisfied;
    }
}
