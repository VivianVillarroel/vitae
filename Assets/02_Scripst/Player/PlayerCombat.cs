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

        Item currentItem = Inventory.instance.selectedItem;
        bool usingSword = currentItem != null && currentItem.itemType == Item.ItemType.Arma;

        // Animación diferente si se usa espada
        if (usingSword)
        {
            animator.SetTrigger("SwordAttack");
        }
        else
        {
            animator.SetTrigger("Attack");
        }

        // Dibuja para depurar
        Debug.DrawRay(attackPoint.position, attackPoint.forward * attackRange, Color.red, 1f);

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        Debug.Log($"[ATAQUE] Enemigos detectados: {hitEnemies.Length}");

        // Aumentar daño si se usa espada
        int extraDamage = 0;
        if (usingSword)
        {
            switch (currentItem.itemName)
            {
                case "Espada de Piedra":
                    extraDamage = 10;
                    break;
                case "Espada SuperTachada":
                    extraDamage = 25;
                    break;
                default:
                    extraDamage = 5;
                    break;
            }
        }

        int totalDamage = attackDamage + extraDamage;

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log($"[ATAQUE] Golpeando a: {enemy.name}", enemy);

            // Atacar enemigo
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                continue;
            }

            // Atacar objetos rompibles
            BreakableObject breakable = enemy.GetComponent<BreakableObject>();
            if (breakable != null)
            {
                Debug.Log($"[ATAQUE] Golpeando objeto rompible: {enemy.name}");
                breakable.TakeDamage(attackDamage);
                continue;
            }


            Debug.LogWarning($"[ATAQUE] {enemy.name} no tiene EnemyHealth ni BreakableObject.");
        }

    }



    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}