using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 3.5f;
    public float detectionRadius = 10f;
    public float attackRange = 3.7f;
    public float wanderRadius = 5f;
    public float idleTime = 3f;

    [Header("Combat")]
    public int attackDamage = 10;
    public float attackCooldown = 2f;
    public float attackDelay = 0.5f;

    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;
    public Transform player;
    public EnemyAttackHitBox attackHitBox;

    private float lastAttackTime;
    private float currentIdleTime;
    private bool isInAttackRange = false;
    private bool isChasingPlayer = false;

    private Vector3 homePosition;
    private EnemyHealth health;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null) Debug.LogError("No se encontró objeto con tag 'Player'");

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        attackHitBox = GetComponentInChildren<EnemyAttackHitBox>();

        homePosition = transform.position;

        if (animator != null) animator.applyRootMotion = false;

        StartCoroutine(WanderRoutine());
    }

    void Update()
    {
        if (health.currentHealth <= 0 || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            isChasingPlayer = true;

            FaceTarget();

            if (distanceToPlayer <= attackRange)
            {
                isInAttackRange = true;
                agent.isStopped = true;
                animator.SetBool("IsRunning", false);

                if (Time.time > lastAttackTime + attackCooldown)
                {
                    StartCoroutine(AttackRoutine());
                }
            }
            else
            {
                isInAttackRange = false;
                agent.isStopped = false;
                agent.speed = runSpeed;
                agent.SetDestination(player.position);
                animator.SetBool("IsRunning", true);
                animator.SetBool("IsWalking", false);
            }
        }
        else if (isChasingPlayer)
        {
            // Jugador se alejó, volver a casa
            isChasingPlayer = false;
            isInAttackRange = false;
            agent.isStopped = false;
            agent.speed = runSpeed;
            agent.SetDestination(homePosition);
            animator.SetBool("IsRunning", true);
            animator.SetBool("IsWalking", false);
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            // Si llegó a casa, empezar patrullaje normal
            animator.SetBool("IsRunning", false);
            if (!IsInvoking(nameof(Wander)))
            {
                Invoke(nameof(Wander), idleTime);
            }
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        if (direction.magnitude == 0) return;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    IEnumerator AttackRoutine()
    {
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDelay);

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null && !playerHealth.isDead &&
            Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    void Wander()
    {
        if (isChasingPlayer || health.currentHealth <= 0) return;

        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius + homePosition;
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.speed = walkSpeed;
            agent.SetDestination(hit.position);
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsRunning", false);
        }
    }

    IEnumerator WanderRoutine()
    {
        while (true)
        {
            if (!isChasingPlayer && agent.remainingDistance < 0.2f)
            {
                Wander();
            }
            yield return new WaitForSeconds(5f);
        }
    }

    public void Die()
    {
        animator.SetTrigger("Die");
        agent.isStopped = true;
        StopAllCoroutines();
        Destroy(gameObject, 3f);
    }
}