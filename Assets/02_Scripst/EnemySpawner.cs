using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnZone
    {
        public Vector3 center;
        public float radius = 20f;
        public int maxEnemies = 5;
        public GameObject[] enemyPrefabs;
    }

    public List<SpawnZone> spawnZones = new List<SpawnZone>();
    public DayNightCycle dayNightCycle;
    public LayerMask obstacleMask;
    public float respawnDelay = 60f; // Tiempo para reaparecer enemigos muertos

    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // Revisar cada 5 segundos
            CleanDeadEnemies();
            SpawnEnemies();
        }
    }

    void SpawnEnemies()
    {
        foreach (var zone in spawnZones)
        {
            int enemiesToSpawn = CalculateEnemiesToSpawn(zone);

            if (enemiesToSpawn > 0)
            {
                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    SpawnSingleEnemy(zone);
                }
            }
            else if (dayNightCycle != null && !dayNightCycle.IsNightTime())
            {
                // Eliminar enemigos extra de noche al llegar el día
                RemoveExtraEnemies(zone);
            }
        }
    }

    int CalculateEnemiesToSpawn(SpawnZone zone)
    {
        int currentEnemies = CountEnemiesInZone(zone);
        int desiredEnemies = zone.maxEnemies;

        // Aumentar enemigos de noche
        if (dayNightCycle != null && dayNightCycle.IsNightTime())
        {
            desiredEnemies *= 2; // Doble de enemigos
        }

        return Mathf.Max(0, desiredEnemies - currentEnemies);
    }

    void SpawnSingleEnemy(SpawnZone zone)
    {
        Vector3 spawnPos = FindValidSpawnPosition(zone);

        if (spawnPos != Vector3.zero && zone.enemyPrefabs.Length > 0)
        {
            GameObject enemyPrefab = zone.enemyPrefabs[Random.Range(0, zone.enemyPrefabs.Length)];
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            activeEnemies.Add(newEnemy);
        }
    }

    void RemoveExtraEnemies(SpawnZone zone)
    {
        List<GameObject> enemiesToRemove = new List<GameObject>();

        foreach (var enemy in activeEnemies)
        {
            if (enemy != null && Vector3.Distance(enemy.transform.position, zone.center) <= zone.radius)
            {
                enemiesToRemove.Add(enemy);
                if (enemiesToRemove.Count >= zone.maxEnemies) break;
            }
        }

        foreach (var enemy in enemiesToRemove)
        {
            Destroy(enemy);
            activeEnemies.Remove(enemy);
        }
    }

    Vector3 FindValidSpawnPosition(SpawnZone zone)
    {
        for (int i = 0; i < 20; i++) // 20 intentos
        {
            Vector3 randomPoint = zone.center + Random.insideUnitSphere * zone.radius;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
            {
                if (!Physics.CheckSphere(hit.position, 1f, obstacleMask))
                {
                    return hit.position;
                }
            }
        }
        return Vector3.zero;
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

    void CleanDeadEnemies()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);
    }
}