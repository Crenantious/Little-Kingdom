using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrefabReference))]
public class PrefabRefererenceEditor : Editor
{
    string[] guids;
    List<string> names = new();
    List<GameObject> gameObjects = new();
    int dropdownMenuSelectedItemIndex = 0;
    private void OnEnable()
    {
        guids = AssetDatabase.FindAssets("t:Prefab", new string[]{"Assets/Prefabs"});

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            names.Add(go.name);
            gameObjects.Add(go);
        }
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        bool buttonClicked = EditorGUILayout.DropdownButton(new GUIContent(text: "Select prefab"), FocusType.Passive);
        if (buttonClicked)
        {
            DisplayDropDown();
        }


        foreach(string name in names)
        {
            EditorGUILayout.LabelField(name);
        }

        EditorGUILayout.ObjectField(PrefabReferenceSO.GetPrefab(((PrefabReference)target).referenceID),
            typeof(GameObject), true);

        serializedObject.ApplyModifiedProperties();
    }

    void DisplayDropDown()
    {
        //dropdownMenuSelectedItemIndex = EditorGUILayout.Popup("Label", dropdownMenuSelectedItemIndex, names);
    }
}
