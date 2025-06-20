using System.Collections;
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
    public float attackAnimationDelay = 0.3f; // Tiempo antes de aplicar daño

    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;
    public Transform player;
    public EnemyAttackHitBox attackHitBox;

    [Header("Attack Settings")]
    public float stoppingDistance = 1.3f; // Distancia para detenerse y atacar
    public float attackDelay = 0.5f; // Tiempo antes de aplicar daño

    private bool isInAttackRange = false;

    private Vector3 homePosition;
    private float currentIdleTime;
    private float lastAttackTime;
    private EnemyHealth health;
    private bool isAgentActive = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) Debug.LogError("No se encontró objeto con tag 'Player'");
        
        
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        attackHitBox = GetComponentInChildren<EnemyAttackHitBox>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        homePosition = transform.position;

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

        // Rotar hacia el jugador cuando está cerca
        if (distanceToPlayer <= detectionRadius)
        {
            FaceTarget(); // Nueva función para mirar al jugador

            if (distanceToPlayer <= stoppingDistance)
            {
                if (!isInAttackRange)
                {
                    isInAttackRange = true;
                    agent.isStopped = true;
                    animator.SetBool("IsRunning", false);
                }

                if (Time.time > lastAttackTime + attackCooldown)
                {
                    StartCoroutine(AttackRoutine());
                }
            }
            else
            {
                if (isInAttackRange)
                {
                    isInAttackRange = false;
                    agent.isStopped = false;
                }

                agent.SetDestination(player.position);
                agent.speed = runSpeed;
                animator.SetBool("IsRunning", true);
            }
        }
        else
        {
            Debug.Log("Jugador fuera de rango de detección");
            agent.speed = walkSpeed;
            animator.SetBool("IsRunning", false);
        }
    }
    void FaceTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    IEnumerator AttackRoutine()
    {
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDelay);

        // Verificar si el jugador sigue vivo y en rango
        PlayerHealth playerHealth = player?.GetComponent<PlayerHealth>();
        if (playerHealth != null && !playerHealth.isDead &&
            Vector3.Distance(transform.position, player.position) <= stoppingDistance * 1.2f)
        {
            playerHealth.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(0.5f);
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

    public void Die()
    {
        animator.SetTrigger("Die");
        if (agent.isOnNavMesh) agent.isStopped = true;
        Destroy(gameObject, 3f);
    }
}