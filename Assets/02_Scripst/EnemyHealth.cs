using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public GameObject[] lootDrops; // Prefabs de items que puede soltar al morir
    public float dropChance = 0.7f; // 70% de chance de dropear algo

    void Start()
    {
        currentHealth = maxHealth;
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
        // Lógica para dropear items
        if (lootDrops != null && lootDrops.Length > 0 && Random.value <= dropChance)
        {
            GameObject itemToDrop = lootDrops[Random.Range(0, lootDrops.Length)];
            Instantiate(itemToDrop, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}