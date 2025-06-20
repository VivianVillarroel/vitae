using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public GameObject[] lootDrops; // Items que suelta al morir
    public float dropChance = 0.7f; // 70% de probabilidad de soltar algo

    private EnemyAI enemyAI;

    void Start()
    {
        currentHealth = maxHealth;
        enemyAI = GetComponent<EnemyAI>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        enemyAI.Die();

        // Soltar loot aleatorio
        if (Random.value <= dropChance && lootDrops.Length > 0)
        {
            Instantiate(lootDrops[Random.Range(0, lootDrops.Length)], transform.position, Quaternion.identity);
        }

        Destroy(gameObject, 3f); // Espera a que termine la animación
    }
}