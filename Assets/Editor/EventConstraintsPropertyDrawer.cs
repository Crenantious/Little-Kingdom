using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PropertyDrawerUtilities;
using static EventConstraintsGameStateAssetPostProcessor;

[CustomPropertyDrawer(typeof(EventConstraints))]
public class EventConstraintPropertyDrawer : PropertyDrawer
{
    const string gameStateHeaderName = "GameState";
    const string eventInfoHeaderName = "EventInfo";

    bool isSetup = false;
    SerializedProperty allowStates;
    SerializedProperty states;
    SerializedProperty gameObjectConstraints;
    GUIStyle headerStyle = new(EditorStyles.label)
    {
        fontSize = 15
    };

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        if (!isSetup)
        {
            allowStates = property.FindPropertyRelative("allowStates");
            states = property.FindPropertyRelative("states");
            gameObjectConstraints = property.FindPropertyRelative("gameObjectConstraints");
            isSetup = true;
        }

        this.BeginProperty(rect, property, label);
        property.serializedObject.Update();

        // GameStates
        EditorGUI.LabelField(this.GetNextRect(height: headerStyle.fontSize), gameStateHeaderName, headerStyle);
        this.VerticalSpace(2);
        DrawStatesMenu();
        this.VerticalSpace(10);
        allowStates.boolValue = EditorGUI.Toggle(this.GetNextRect(), "Allow selected states", allowStates.boolValue);

        // GameObjects
        EditorGUI.LabelField(this.GetNextRect(), eventInfoHeaderName, headerStyle);
        EditorGUI.PropertyField(this.GetNextRect(gameObjectConstraints), gameObjectConstraints);

        property.serializedObject.ApplyModifiedProperties();
        this.EndProperty(property);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
        this.GetActivePropertyHeight(property);

    void DrawStatesMenu()
    {
        this.MakeNextRectsHorizontal(2);

        if (GUI.Button(this.GetNextRect(width:60), "Invert"))
            InvertGameStates();

        string notAllowedText = allowStates.boolValue ? "" : "not ";
        if (EditorGUI.DropdownButton(this.GetNextRect(), 
                                     new GUIContent(text: $"Select {notAllowedText}allowed {nameof(GameState)}s"), 
                                     FocusType.Passive))
        {
            GenericMenu allowedGameStatesMenu = new();
            List<string> names = GetGameStateNames();

            for (int i = 0; i < names.Count; i++)
            {
                string name = names[i];
                allowedGameStatesMenu.AddItem(new GUIContent(name), 
                                              GetIndexInStates(GetGameState(name)) != -1, 
                                              GameStatesMenuCallback, name);
            }
            allowedGameStatesMenu.ShowAsContext();
        }
    }

    /// <returns>The index of the gameState in <see cref="states"/>, or -1 if it is not found.</returns>
    int GetIndexInStates(GameState gameState)
    {
        for (int i = 0; i < states.arraySize; i++)
        {
            if (states.GetArrayElementAtIndex(i).objectReferenceValue == gameState) 
                return i;
        }
        return -1;
    }

    /// <summary>
    /// If a <see cref="GameState"/> is in <see cref="states"/> it will be removed. If it is not, it will added.
    /// </summary>
    void InvertGameStates()
    {
        List<GameState> gameStates = GetGameStates();
        List<GameState> previousStates = new();

        for (int i = 0; i < states.arraySize; i++)
            previousStates.Add((GameState)states.GetArrayElementAtIndex(i).objectReferenceValue);
        states.arraySize = 0;

        for (int i = 0; i < gameStates.Count; i++)
        {
            if (!previousStates.Contains(gameStates[i]))
            {
                states.arraySize++;
                states.GetArrayElementAtIndex(states.arraySize - 1).objectReferenceValue = gameStates[i];
            }
        }
    }

    void GameStatesMenuCallback(object nameObject)
    {
        string name = (string)nameObject;
        states.serializedObject.Update();

        GameState gameState = GetGameState(name);
        if (!gameState)
            throw new System.NullReferenceException($"Selected {nameof(GameState)} {name} cannot be found.");

        int index = GetIndexInStates(gameState);

        if (index != -1)
            states.DeleteArrayElementAtIndex(index);
        else
        {
            states.arraySize++;
            states.GetArrayElementAtIndex(states.arraySize - 1).objectReferenceValue = gameState;
        }
        states.serializedObject.ApplyModifiedProperties();
    }
}