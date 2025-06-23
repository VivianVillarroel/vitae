using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [Header("Vida del objeto")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Referencia opcional")]
    public Animator animator;

    private bool isBroken = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        Debug.Log($"[BreakableObject] Inicializado en {gameObject.name} con {maxHealth} de vida.");
    }

    public void TakeDamage(int damage)
    {
        if (isBroken)
        {
            Debug.LogWarning($"[BreakableObject] {gameObject.name} ya está destruido, ignorando daño.");
            return;
        }

        Debug.Log($"[BreakableObject] {gameObject.name} recibió {damage} de daño.");

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Break();
        }
        else if (animator != null && HasParameter("Hit", animator))
        {
            animator.SetTrigger("Hit");
        }
    }

    void Break()
    {
        isBroken = true;
        Debug.Log($"[BreakableObject] {gameObject.name} ha sido destruido.");

        if (animator != null && HasParameter("Break", animator))
        {
            animator.SetTrigger("Break");
        }

        ItemDrop drop = GetComponent<ItemDrop>();
        if (drop != null)
        {
            Debug.Log("[BreakableObject] Ejecutando DropItem...");
            drop.DropItem();
        }
        else
        {
            Debug.LogWarning("[BreakableObject] No se encontró ItemDrop.");
        }

        Destroy(gameObject, 2f);
    }

    bool HasParameter(string paramName, Animator anim)
    {
        foreach (var param in anim.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }
}
