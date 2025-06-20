using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public int attackDamage = 20;
    public float attackCooldown = 1f;

    public LayerMask enemyLayer;
    public Animator animator;
    private float nextAttackTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        // Asigna automáticamente el layer "Enemy" (layer número 9)
        enemyLayer = 1 << 3; // Si "Enemy" es el layer 9
    }

    void Update()
    {
        if (Time.time >= nextAttackTime && Input.GetMouseButtonDown(0))
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Attack()
    {
        Debug.Log("[ATAQUE] Iniciando ataque...");
        animator.SetTrigger("Attack");

        // Depuración: Dibuja el área de ataque
        Debug.DrawRay(attackPoint.position, attackPoint.forward * attackRange, Color.red, 1f);

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        Debug.Log($"[ATAQUE] Enemigos detectados: {hitEnemies.Length}");

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log($"[ATAQUE] Golpeando a: {enemy.name}", enemy);

            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
            else
            {
                Debug.LogError($"[ATAQUE] {enemy.name} no tiene EnemyHealth!", enemy);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}