using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

[InitializeOnLoad]
public class EventConstraintsGameStateAssetPostProcessor : AssetPostprocessor
{
    public static readonly string[] pathsToSearch = new string[] { "Assets/Scripts/Game states" };

    static List<string> gameStateNames = new();
    static List<GameState> gameStates = new();
    static List<string> toBeDeleted = new();

    static EventConstraintsGameStateAssetPostProcessor()=>
        GetAllGameStates();

    /// <summary>
    /// Attempts to load a <see cref="GameState"/> object in one of the allowed <see cref="pathsToSearch">paths</see>.
    /// </summary>
    /// <returns>The loaded <see cref="GameState"/>, or null if it could not be found.</returns>
    public static GameState GetGameState(string name)
    {
        GameState gameState = null;
        foreach (string path in pathsToSearch)
        {
            gameState = AssetDatabase.LoadAssetAtPath<GameState>($"{path}/{name}.asset");
            if (gameState)
                break;
        }
        return gameState;
    }

    public static List<string> GetGameStateNames() =>
        gameStateNames;

    public static List<GameState> GetGameStates() =>
        gameStates;

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        toBeDeleted.Clear();

        foreach (string path in deletedAssets.Union(movedFromAssetPaths))
        {
            if (!IsValidPath(path))
                continue;
            toBeDeleted.Add(path);
        }

        foreach (string path in importedAssets.Union(movedAssets))
        {
            GameState gameState = LoadAsset(path);
            if (!gameState)
                continue;

            int index = gameStates.IndexOf(gameState);

            if (IsValidPath(path))
            {
                if (index == -1)
                    AddNewGameState(gameState);
                else
                    gameStateNames[index] = GetName(path);
            }
            else
            {
                if (index != -1)
                    RemoveAt(index);
            }
        }

        foreach(string path in toBeDeleted)
        {
            // If the path does not exist in gameStateNames then it was simply renamed
            int index = gameStateNames.IndexOf(GetName(path));
            if (index != -1)
                RemoveAt(index);
        }
    }

    static void GetAllGameStates()
    {
        gameStateNames.Clear();
        gameStates.Clear();

        foreach (string guid in AssetDatabase.FindAssets("t:GameState", pathsToSearch))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AddNewGameState(path);
        }
    }

    static void AddNewGameState(string path)
    {
        gameStateNames.Add(GetName(path));
        gameStates.Add(LoadAsset(path));
    }

    static void AddNewGameState(GameState gameState)
    {
        gameStateNames.Add(gameState.name);
        gameStates.Add(gameState);
    }

    static void RemoveAt(int index)
    {
        RemoveFromAllEventConstraintsMonoInstances(gameStates[index]);

        gameStateNames.RemoveAt(index);
        gameStates.RemoveAt(index);
    }


    /// <summary>
    /// Searches all instances of the <see cref="EventConstraintsMono"/> component and removes any null references to
    /// <see cref="GameState"/> in the "states" field.
    /// </summary>
    static void RemoveFromAllEventConstraintsMonoInstances(GameState gameState = null)
    {
        EventConstraintsMono[] components = Resources.FindObjectsOfTypeAll<EventConstraintsMono>();
        bool isNull = gameState == null;

        foreach (EventConstraintsMono component in components)
        {
            List<GameState> states = component.eventConstraints.states;
            for (int i = 0; i < states.Count; i++)
            {
                if (isNull && !states[i] || states[i] == gameState)
                    states.RemoveAt(i);
            }
        }
    }

    /// <param name="path">The path of a <see cref="GameState"/> asstet.</param>
    /// <returns>True if the directory of the given path is in <see cref="pathsToSearch">paths</see>.</returns>
    static bool IsValidPath(string path) =>
         pathsToSearch.Contains(Path.GetDirectoryName(path).Replace("\\", "/"));

    static GameState LoadAsset(string path) => 
        AssetDatabase.LoadAssetAtPath<GameState>(path);

    static string GetName(string path) =>
        Path.GetFileNameWithoutExtension(path);
    //static void AddGameState(GameState gameState)
    //{
    //    EventConstraintsMono[] components = Resources.FindObjectsOfTypeAll<EventConstraintsMono>();

    //    foreach (var component in components)
    //    {
    //        if (component.eventConstraints.statesAllowed)
    //            component.eventConstraints.states.Add(gameState);
    //    }
    //}
}
