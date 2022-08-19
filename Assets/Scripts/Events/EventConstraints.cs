using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EventConstraints
{
    public bool allowStates = false;
    public List<GameState> states = new();
    public List<GameObjectConstraint> gameObjectConstraints = new();

    [Serializable]
    public enum GameObjectConstraintType
    {
        InstanceOfPrefab = 1,
        HasComponent = 2
    }

    [Serializable]
    public struct GameObjectConstraint
    {
        public GameObjectConstraintType constraint;
        public GameObject gameObject;
        public Type componentType;

        public GameObjectConstraint(GameObjectConstraintType constraint, GameObject gameObject = default,
                                    Type componentType = default)
        {
            this.gameObject = gameObject;
            this.constraint = constraint;
            this.componentType = componentType;
        }
    }
}
