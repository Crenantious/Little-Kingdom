using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoardManager))]
public class BoardManagerEditor : Editor
{
    public SerializedProperty tilePercentages;
    public SerializedProperty tilesParent;

    void OnEnable()
    {
        tilePercentages = serializedObject.FindProperty("tilePercentages");
        tilesParent = serializedObject.FindProperty("tilesParent");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(tilesParent);
        EditorGUILayout.LabelField("Total percent: " + GetTotalPercent().ToString());

        DisplayTilesAndPercentages();
        DisplayAndHandleButtons();

        serializedObject.ApplyModifiedProperties();
    }
    void DisplayAndHandleButtons()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add tile"))
        {
            tilePercentages.arraySize++;
        }

        if (GUILayout.Button("Remove tile") && tilePercentages.arraySize > 0)
        {
            tilePercentages.arraySize--;
        }

        EditorGUILayout.EndHorizontal();
    }
    int GetTotalPercent()
    {
        int totalPercent = 0;
        for (int i = 0; i < tilePercentages.arraySize; i++)
        {
            totalPercent += ((BoardManager.TileTypeValue)tilePercentages.GetArrayElementAtIndex(i).boxedValue).value;
        }
        return totalPercent;
    }

    void DisplayTilesAndPercentages()
    {
        for (int i = 0; i < tilePercentages.arraySize; i++)
        {
            BoardManager.TileTypeValue tileTypePercent = (BoardManager.TileTypeValue)tilePercentages.GetArrayElementAtIndex(i).boxedValue;

            GUILayout.BeginHorizontal();
            TileType tileType = (TileType)EditorGUILayout.ObjectField(tileTypePercent.tileType, typeof(TileType), false);
            int tilePercent = EditorGUILayout.IntField(tileTypePercent.value);
            GUILayout.EndHorizontal();

            tilePercent = Mathf.Clamp(tilePercent, 0, 100);
            tilePercentages.GetArrayElementAtIndex(i).boxedValue = new BoardManager.TileTypeValue(tileType, tilePercent);
        }
    }
}
