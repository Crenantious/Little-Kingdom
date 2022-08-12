using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EventConstraints
{
    public List<GameState> statesAllowed = new();
    public List<GameState> statesNotAllowed = new();

    public List<GameObjectConstraint> gameObjectConstraints;

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


    [Serializable]
    public enum GameObjectConstraintType
    {
        InstanceOfPrefab = 1
        //DirectParent = 2,
        //DirectChild = 4,
        //Parent = 8,
        //Child = 16,
    }
}
