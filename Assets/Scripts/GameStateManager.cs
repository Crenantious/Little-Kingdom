using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    /// <value>50</value>
    const int maxPreviousStates = 50;

    public GameState initialState;
    readonly List<GameState> previousStates = new();

    [field: SerializeField] public GameState CurrentSate { get; private set; }


    private void Awake()
    {
        if (!initialState)
            throw new UnassignedReferenceException($"Must set an initial {typeof(GameState)}");
        CurrentSate = initialState;
    }

    /// <summary>
    /// Adds the current state to a list of previous states (max <inheritdoc cref="maxPreviousStates" path="//value"/>), 
    /// then sets the current state to the given state.
    /// </summary>
    public void SetState(GameState state)
    {
        previousStates.Add(CurrentSate);
        if (previousStates.Count == maxPreviousStates + 1)
            previousStates.RemoveAt(0);
        CurrentSate = state;
    }

    /// <summary>
    /// Sets the current state to the last state in the list of previous states, then removes that state from the list
    /// </summary>
    public void RevertState()
    {
        if (previousStates.Count == 0)
            return;

        CurrentSate = previousStates[^1];
        previousStates.RemoveAt(previousStates.Count - 1);
    }
}
