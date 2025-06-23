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
        public float radius = 10f;
        public int maxEnemies = 3;
        public GameObject[] enemyPrefabs;
    }

    public List<SpawnZone> spawnZones = new List<SpawnZone>();
    public LayerMask obstacleMask;

    private List<GameObject> activeEnemies = new List<GameObject>();

    IEnumerator Start()
    {
        // Esperar un frame para que el NavMesh esté listo
        yield return null;

        while (true)
        {
            CleanDeadEnemies();
            SpawnEnemies();
            yield return new WaitForSeconds(10f); // Revisar cada 10 segundos
        }
    }

    void SpawnEnemies()
    {
        foreach (var zone in spawnZones)
        {
            int enemiesInZone = CountEnemiesInZone(zone);

            if (enemiesInZone < zone.maxEnemies)
            {
                StartCoroutine(SpawnEnemyWithDelay(zone, zone.maxEnemies - enemiesInZone));
            }
        }
    }

    IEnumerator SpawnEnemyWithDelay(SpawnZone zone, int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            SpawnSingleEnemy(zone);
            yield return new WaitForSeconds(2f); // Espera entre spawns
        }
    }

    void SpawnSingleEnemy(SpawnZone zone)
    {
        Vector3 spawnPos = FindValidSpawnPosition(zone);

        if (spawnPos != Vector3.zero && zone.enemyPrefabs.Length > 0)
        {
            GameObject enemyPrefab = zone.enemyPrefabs[Random.Range(0, zone.enemyPrefabs.Length)];
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            activeEnemies.Add(newEnemy);

            // Asegurar que el NavMeshAgent funciona
            StartCoroutine(InitializeNavMeshAgent(newEnemy));
        }
    }

    Vector3 FindValidSpawnPosition(SpawnZone zone)
    {
        for (int i = 0; i < 30; i++) // 30 intentos
        {
            Vector3 randomPoint = zone.center + Random.insideUnitSphere * zone.radius;
            randomPoint.y = 0;

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

    IEnumerator InitializeNavMeshAgent(GameObject enemy)
    {
        yield return new WaitForEndOfFrame();

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null && !agent.isOnNavMesh)
        {
            agent.Warp(enemy.transform.position);
        }
    }

    int CountEnemiesInZone(SpawnZone zone)
    {
        int count = 0;
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null &&
                Vector3.Distance(enemy.transform.position, zone.center) <= zone.radius)
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        foreach (var zone in spawnZones)
        {
            Gizmos.DrawWireSphere(zone.center, zone.radius);
        }
    }
}