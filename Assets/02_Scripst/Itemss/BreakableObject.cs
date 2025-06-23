using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;

    public GameObject dropItemPrefab; // lo que va a soltar (ej: piedra)
    public Transform dropPoint;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"[BREAKABLE] {gameObject.name} recibió {damage} de daño");

        if (currentHealth <= 0)
        {
            Break();
        }
    }

    void Break()
    {
        Debug.Log($"[BREAKABLE] {gameObject.name} destruido");

        if (dropItemPrefab != null && dropPoint != null)
        {
            Instantiate(dropItemPrefab, dropPoint.position, Quaternion.identity);
        }

        Destroy(gameObject); // o animación de destrucción
    }
}
