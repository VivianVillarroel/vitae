using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnZone
    {
        public Vector3 center;
        public float radius = 100f;
        public int maxEnemies = 5;
        public GameObject[] enemyPrefabs;
    }

    public List<SpawnZone> spawnZones = new List<SpawnZone>();
    public DayNightCycle dayNightCycle;
    public WeatherSystem weatherSystem;

    [Header("Spawn Settings")]
    public int baseNightEnemiesMultiplier = 2;
    public float spawnCheckInterval = 30f;
    public float respawnDelay = 60f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private float lastSpawnCheckTime;

    void Start()
    {
        InitializeSpawnZones();
        lastSpawnCheckTime = -spawnCheckInterval; // Para que se ejecute inmediatamente
    }

    void InitializeSpawnZones()
    {
        // Puedes inicializar zonas predeterminadas aquí o configurarlas en el Inspector
        if (spawnZones.Count == 0)
        {
            spawnZones.Add(new SpawnZone()
            {
                center = new Vector3(252.5f, 2.13f, 237.5f),
                radius = 100f,
                maxEnemies = 5
            });

            spawnZones.Add(new SpawnZone()
            {
                center = new Vector3(322f, 2.13f, 720f),
                radius = 100f,
                maxEnemies = 5
            });

            spawnZones.Add(new SpawnZone()
            {
                center = new Vector3(704.5f, 2.13f, 736f),
                radius = 100f,
                maxEnemies = 5
            });
        }
    }

    void Update()
    {
        if (Time.time - lastSpawnCheckTime >= spawnCheckInterval)
        {
            lastSpawnCheckTime = Time.time;
            CheckEnemyCount();
        }
    }

    void CheckEnemyCount()
    {
        // Limpiar enemigos muertos
        activeEnemies.RemoveAll(enemy => enemy == null);

        foreach (var zone in spawnZones)
        {
            int currentEnemiesInZone = CountEnemiesInZone(zone);
            int desiredEnemies = CalculateDesiredEnemies(zone);

            if (currentEnemiesInZone < desiredEnemies)
            {
                SpawnEnemies(zone, desiredEnemies - currentEnemiesInZone);
            }
        }
    }

    int CountEnemiesInZone(SpawnZone zone)
    {
        int count = 0;
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null && Vector3.Distance(enemy.transform.position, zone.center) <= zone.radius)
            {
                count++;
            }
        }
        return count;
    }

    int CalculateDesiredEnemies(SpawnZone zone)
    {
        int baseAmount = zone.maxEnemies;

        if (dayNightCycle != null && dayNightCycle.IsNightTime())
        {
            baseAmount *= baseNightEnemiesMultiplier;
        }

        if (weatherSystem != null && weatherSystem.isRaining)
        {
            baseAmount = Mathf.RoundToInt(baseAmount * 1.5f);
        }

        return baseAmount;
    }

    void SpawnEnemies(SpawnZone zone, int amount)
    {
        if (zone.enemyPrefabs == null || zone.enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No enemy prefabs assigned to spawn zone!");
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPos = zone.center + Random.insideUnitSphere * zone.radius;
            spawnPos.y = zone.center.y; // Mantener la misma altura

            GameObject enemyPrefab = zone.enemyPrefabs[Random.Range(0, zone.enemyPrefabs.Length)];
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            activeEnemies.Add(newEnemy);

            // Configurar el enemigo (añadir scripts de comportamiento, etc.)
            SetupEnemy(newEnemy);
        }
    }

    void SetupEnemy(GameObject enemy)
    {
        // Aquí puedes añadir componentes comunes a todos los enemigos
        if (!enemy.GetComponent<EnemyHealth>())
        {
            enemy.AddComponent<EnemyHealth>();
        }

        // Añadir más configuraciones según necesites
    }
}