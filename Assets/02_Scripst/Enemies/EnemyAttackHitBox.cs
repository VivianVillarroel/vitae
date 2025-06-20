using UnityEngine;

public class EnemyAttackHitBox : MonoBehaviour
{
    public int damage = 10;
    public float pushForce = 3f;
    public Collider hitBoxCollider;
    private EnemyAI enemyAI;

    void Start()
    {
        hitBoxCollider = GetComponent<Collider>();
        enemyAI = GetComponentInParent<EnemyAI>();
        DisableHitBox(); // Comienza desactivado
    }

    public void EnableHitBox()
    {
        hitBoxCollider.enabled = true;
    }

    public void DisableHitBox()
    {
        hitBoxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"HitBox activado. Objeto: {other.name}, Tag: {other.tag}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador detectado en HitBox");
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log("Aplicando daño al jugador");
                playerHealth.TakeDamage(damage);
            }
            else
            {
                Debug.LogError("No se encontró PlayerHealth en el jugador");
            }
        }
    }
}