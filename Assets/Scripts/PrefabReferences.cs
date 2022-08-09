using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PrefabReferencesAssetPostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        for (var i = 0; i < importedAssets.Length; i++)
        {
            GameObject prefab = GetPrefab(importedAssets[i]);

            if (prefab != null)
            {
                PrefabReferenceSO.PrefabCreated(prefab);
            }
        }

        for (var i = 0; i < deletedAssets.Length; i++)
        {
            PrefabReferenceSO.CheckAndCreateAsset();
            //GameObject prefab = GetPrefab(deletedAssets[i]);

            //if (prefab != null)
            //{
            //    PrefabReferenceSO.PrefabDeleted(prefab);
            //}
        }
    }

    static GameObject GetPrefab(string name)
    {
        Object asset = AssetDatabase.LoadAssetAtPath<Object>(name);
        if (asset.GetType() == typeof(GameObject))
        {
            return (GameObject)asset;
        }
        return null;
    }
}
