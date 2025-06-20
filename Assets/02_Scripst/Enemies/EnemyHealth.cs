using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 50;
    public int currentHealth;
    public NavMeshAgent agent;
    public Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        agent.isStopped = true; // Detiene el movimiento
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Invoke("ResumeMovement", 1f); // Reanuda después de 1 segundo
        }
    }

    void ResumeMovement()
    {
        agent.isStopped = false;
    }

    void Die()
    {
        agent.enabled = false;
        animator.SetTrigger("Die");
        Destroy(gameObject, 3f);
    }
}