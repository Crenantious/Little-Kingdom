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
        InstanceOfPrefab = 1
    }

    [Serializable]
    public struct GameObjectConstraint
    {
        public GameObjectConstraint(GameObject gameObject, GameObjectConstraintType constraint)
        {
            this.gameObject = gameObject;
            this.constraint = constraint;
        }
        public GameObject gameObject;
        public GameObjectConstraintType constraint;
    }
}
