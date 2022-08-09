using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Linq;

[InitializeOnLoad]
public class PrefabReferenceSO : ScriptableObject
{
    const string instancePath = "Assets/Scripts/PrefabReferenceSO.asset";
    const string prefabFolderPath = "Assets/Prefabs";
    static PrefabReferenceSO instance;

    Dictionary<int, GameObject> prefabMap = new();
    [SerializeField] List<PrefabAndID> prefabAndIDs = new();
    int currentID = 0;

    [System.Serializable]
    struct PrefabAndID
    {
        public PrefabAndID(int ID, GameObject Prefab)
        {
            this.ID = ID;
            this.Prefab = Prefab;
        }
        public int ID;
        public GameObject Prefab;
    }

    static PrefabReferenceSO()
    {
        CheckAndCreateAsset();
    }

    public void OnEnable()
    {
        if (AssetDatabase.GetAssetPath(this).EndsWith(instancePath))
        {
            return;
        }

        instance = this;

        // Initialise the Dictionary since it not Serializeable
        foreach (PrefabAndID prefabAndID in prefabAndIDs)
        {
            prefabMap[prefabAndID.ID] = prefabAndID.Prefab;
        }
    }

    public static void PrefabCreated(GameObject prefab)
    {
        AddPrefabReference(prefab);
    }

    static void PrefabDeleted(GameObject prefab)
    {
        int index = ((List<GameObject>)instance.prefabAndIDs.Select(x => x.ID)).IndexOf(prefab);
        int key = instance.prefabAndIDs[index].ID;

        if (index != -1)
        {
            if (instance.prefabMap.ContainsKey(key))
            {
                instance.prefabMap.Remove(key);
            }
            instance.prefabAndIDs.RemoveAt(key);
        }

    }

    public static GameObject GetPrefab(int ID)
    {
        if (instance == null) CheckAndCreateAsset();
        if (instance.prefabMap.ContainsKey(ID)) return instance.prefabMap[ID];
        return null;
    }

    /// <summary>
    /// Stores the given prefab with a referece ID to be used in the <see cref="PrefabReference"> Component
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns>The ID associated with the prefab or -1 the if prefab reference already exists</returns>
    static int AddPrefabReference(GameObject prefab)
    {
        if (instance == null) CheckAndCreateAsset();
        if (instance.prefabMap.ContainsValue(prefab)) return -1;

        instance.prefabAndIDs.Add(new (instance.currentID, prefab));
        instance.prefabMap[instance.currentID] = prefab;

        EditorUtility.SetDirty(instance);
        TryAddReferenceComponent(prefab, instance.currentID);

        instance.currentID++;
        return instance.currentID - 1;
    }
    static void TryAddReferenceComponent(GameObject prefab, int ID)
    {
        if (!prefab.TryGetComponent(out PrefabReference prefabReferenceComponenet))
        {
            prefabReferenceComponenet = prefab.AddComponent<PrefabReference>();
        }

        prefabReferenceComponenet.referenceID = ID;
    }
    public static void CheckAndCreateAsset()
    {
        PrefabReferenceSO prefabReferenceStorage = AssetDatabase.LoadAssetAtPath<PrefabReferenceSO>(instancePath);
        if (prefabReferenceStorage == null)
        {
            instance = CreateInstance<PrefabReferenceSO>();

            AssetDatabase.CreateAsset(instance, instancePath);
            AssetDatabase.SaveAssets();

            GenerateReferencesForAllPrefabs();
        }
    }

    static void GenerateReferencesForAllPrefabs()
    {
        foreach(GameObject prefab in AssetDatabase.LoadAllAssetsAtPath(prefabFolderPath))
        {
            Debug.Log(prefab.name);
            AddPrefabReference(prefab);
        }
    }
}
