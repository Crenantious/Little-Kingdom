using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class PrefabReferencesManager : AssetPostprocessor
{
    const string prefabFolderPath = "Assets/Prefabs";

    static PrefabReferencesManager()
    {
        GenerateReferencesForAllPrefabsInFolders(new string[] { prefabFolderPath });
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        for (var i = 0; i < importedAssets.Length; i++)
        {
            if (!importedAssets[i].StartsWith(prefabFolderPath)) continue;

            GameObject prefab = GetPrefab(importedAssets[i]);

            if (prefab != null)
                TryAddReferenceComponent(prefab);
        }
    }

    static GameObject GetPrefab(string path)
    {
        Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
        if (asset.GetType() == typeof(GameObject))
            return (GameObject)asset;

        return null;
    }

    static void GenerateReferencesForAllPrefabsInFolders(string[] folderPaths)
    {
        foreach (string guid in AssetDatabase.FindAssets("t:prefab", folderPaths))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            TryAddReferenceComponent(prefab);
        }
    }

    static void TryAddReferenceComponent(GameObject prefab)
    {
        if (!prefab.TryGetComponent(out PrefabReference prefabReferenceComponenet))
        {
            prefabReferenceComponenet = prefab.AddComponent<PrefabReference>();
        }

        if (prefabReferenceComponenet.GUID == "")
        {
            prefabReferenceComponenet.GUID = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(prefab);
        }
    }
}