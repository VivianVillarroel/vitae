using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage = 10;
    public float pushForce = 3f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    private void OnTriggerStay(Collider other)
    {
        // Solo ataca si es el jugador y ha pasado el tiempo de espera
        if (other.CompareTag("Player") && Time.time > lastAttackTime + attackCooldown)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);

                // Empuja al jugador
                Vector3 pushDirection = (other.transform.position - transform.position).normalized;
                other.GetComponent<CharacterController>().Move(pushDirection * pushForce * Time.deltaTime);

                lastAttackTime = Time.time;
            }
        }
    }
}