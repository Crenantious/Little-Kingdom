using UnityEditor;
using UnityEngine;

public static class EventConstraintsTestUtilities
{
    static GameObject testObject;
    static GameStateManager gameStateManager;

    public static void SetupReferences()
    {
        testObject = Object.Instantiate(GetTestPrefab(1));
        gameStateManager = testObject.AddComponent<GameStateManager>();
        References.gameStateManager = gameStateManager;
    }

    public static void TearDownReferences()
    {
        if (testObject) Object.DestroyImmediate(testObject);
        if (gameStateManager) Object.DestroyImmediate(gameStateManager);
    }

    public static GameObject GetTestPrefab(int number)
    {
        string path = $"Assets/Prefabs/Tests/EventConstraintsTest{number}.prefab";
        GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        if (!gameObject)
            throw new System.NullReferenceException($"Cannot load prefab at {path}. Make sure it exists for the test to run.");
        return gameObject;
    }

    public static Event GetTestEvent() =>
        AssetDatabase.LoadAssetAtPath<Event>("Assets/Tests/TestEvent.asset");
}
