using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 3.5f;
    public float detectionRadius = 10f;
    public float attackRange = 1.5f;
    public float wanderRadius = 5f;
    public float idleTime = 3f;

    [Header("Combat")]
    public int attackDamage = 10;
    public float attackCooldown = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;
    private Vector3 homePosition;
    private float currentIdleTime;
    private float lastAttackTime;
    private EnemyHealth health;
    private bool isAgentActive = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();

        // Buscar al jugador de manera segura
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        homePosition = transform.position;

        // Verificar si el agente está en NavMesh
        if (agent != null && agent.isOnNavMesh)
        {
            isAgentActive = true;
            agent.speed = walkSpeed;
        }
        else
        {
            Debug.LogWarning("NavMeshAgent no está en NavMesh: " + gameObject.name);
            isAgentActive = false;
        }
    }

    void Update()
    {
        if (!isAgentActive || health.currentHealth <= 0) return;

        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(player.position);
                agent.speed = runSpeed;
                animator.SetBool("IsRunning", true);

                if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
        }
        else
        {
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
        if (!agent.isOnNavMesh) return;

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

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    public void Die()
    {
        animator.SetTrigger("Die");
        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
        Destroy(gameObject, 3f);
    }
}