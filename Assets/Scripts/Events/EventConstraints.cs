using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EventConstraints
{
    public List<GameState> statesAllowed = new();
    public List<GameState> statesNotAllowed = new();

    public GameObjectConstraint gameObjectConstraint;

    [Serializable]
    public struct GameObjectConstraint
    {
        public GameObject gameObject;
        public GameObjectConstraintType constraint;
    }


    [Serializable]
    public enum GameObjectConstraintType
    {
        DirectParent,
        DirectChild,
        Parent,
        Child,
        InstanceOfPrefab
    }
}
