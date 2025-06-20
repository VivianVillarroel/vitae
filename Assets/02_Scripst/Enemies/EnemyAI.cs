using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 3.5f; // +10% más rápido al perseguir
    public float detectionRadius = 10f; // Radio para detectar al jugador
    public float attackRange = 1.5f; // Distancia para atacar
    public float wanderRadius = 5f; // Zona de deambulación
    public float idleTime = 3f; // Tiempo en Idle antes de moverse

    [Header("Combat")]
    public int attackDamage = 10;
    public float attackCooldown = 2f;

    public NavMeshAgent agent;
    public Animator animator;
    public Transform player;
    private Vector3 homePosition;
    private float currentIdleTime;
    private float lastAttackTime;
    public EnemyHealth health;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        homePosition = transform.position;
        health = GetComponent<EnemyHealth>();
        agent.speed = walkSpeed;
    }

    void Update()
    {
        if (health.currentHealth <= 0) return; // No hacer nada si está muerto

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Perseguir al jugador si está cerca
        if (distanceToPlayer <= detectionRadius)
        {
            agent.SetDestination(player.position);
            agent.speed = runSpeed;
            animator.SetBool("IsRunning", true);

            // Atacar si está en rango
            if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
        else
        {
            // Volver a deambular si el jugador se aleja
            agent.speed = walkSpeed;
            animator.SetBool("IsRunning", false);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                currentIdleTime += Time.deltaTime;
                animator.SetBool("IsWalking", false);

                if (currentIdleTime >= idleTime)
                {
                    Wander();
                    currentIdleTime = 0f;
                }
            }
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void Wander()
    {
        Vector3 randomPos = Random.insideUnitSphere * wanderRadius + homePosition;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            animator.SetBool("IsWalking", true);
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;

        // Daño al jugador si está en rango
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }
    }

    public void Die()
    {
        animator.SetTrigger("Die");
        agent.isStopped = true;
        Destroy(gameObject, 3f); // Eliminar después de la animación
    }
}