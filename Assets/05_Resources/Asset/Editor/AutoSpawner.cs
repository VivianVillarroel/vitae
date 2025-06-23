#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AutoSpawner : EditorWindow
{
    Terrain terrain;

    [System.Serializable]
    public class SpawnData
    {
        public GameObject prefab;
        public int quantity;
    }

    SerializedObject serializedObject;
    SerializedProperty spawnListProp;

    [SerializeField]
    List<SpawnData> spawnList = new List<SpawnData>();

    float minDistance = 5f;

    [MenuItem("Herramientas/Auto Spawner de Árboles y Piedras")]
    public static void ShowWindow()
    {
        GetWindow<AutoSpawner>("Auto Spawner");
    }

    void OnEnable()
    {
        serializedObject = new SerializedObject(this);
        spawnListProp = serializedObject.FindProperty("spawnList");
    }

    void OnGUI()
    {
        GUILayout.Label("🌲 Generador de Objetos en el Terreno", EditorStyles.boldLabel);

        terrain = (Terrain)EditorGUILayout.ObjectField("Terreno", terrain, typeof(Terrain), true);

        serializedObject.Update();
        EditorGUILayout.PropertyField(spawnListProp, new GUIContent("Objetos a generar"), true);
        serializedObject.ApplyModifiedProperties();

        minDistance = EditorGUILayout.FloatField("Distancia mínima entre objetos", minDistance);

        if (terrain == null)
        {
            EditorGUILayout.HelpBox("⚠️ Asigna el Terrain.", MessageType.Warning);
            return;
        }

        if (spawnList.Count == 0)
        {
            EditorGUILayout.HelpBox("⚠️ Agrega al menos un prefab y cantidad.", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("🧠 Generar Objetos"))
        {
            GenerateObjects();
        }
    }

    void GenerateObjects()
    {
        Transform parent = new GameObject("ObjetosGenerados").transform;
        Vector3 terrainPos = terrain.transform.position;
        TerrainData data = terrain.terrainData;

        float terrainWidth = data.size.x;
        float terrainLength = data.size.z;

        foreach (var spawn in spawnList)
        {
            if (spawn.prefab == null || spawn.quantity <= 0) continue;

            int placed = 0;
            int attempts = 0;

            while (placed < spawn.quantity && attempts < spawn.quantity * 20)
            {
                float x = Random.Range(0, terrainWidth);
                float z = Random.Range(0, terrainLength);
                float y = terrain.SampleHeight(new Vector3(x, 0, z)) + terrainPos.y;

                Vector3 pos = new Vector3(x, y, z) + terrainPos;

                if (IsFarEnough(pos, parent, minDistance))
                {
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(spawn.prefab);
                    obj.transform.position = pos;
                    obj.transform.SetParent(parent);
                    placed++;
                }

                attempts++;
            }

            Debug.Log($"✅ {spawn.prefab.name}: {placed} instanciados.");
        }
    }

    bool IsFarEnough(Vector3 pos, Transform parent, float minDist)
    {
        foreach (Transform child in parent)
        {
            if (Vector3.Distance(pos, child.position) < minDist)
                return false;
        }
        return true;
    }
}
#endif
