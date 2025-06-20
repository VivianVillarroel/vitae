using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    public int currentHealth;
    public bool isDead = false;

    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"[DAÑO] Enemigo recibió {damage} de daño. Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (animator != null && HasParameter("Hit", animator))
        {
            animator.SetTrigger("Hit");
        }
    }

    void Die()
    {
        isDead = true;
        if (agent != null) agent.enabled = false;

        if (animator != null && HasParameter("Die", animator))
        {
            animator.SetTrigger("Die");
        }

        Destroy(gameObject, 3f);
    }

    bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }
}