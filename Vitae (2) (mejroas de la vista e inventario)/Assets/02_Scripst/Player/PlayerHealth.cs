using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Text healthText;
    public GameObject deathScreen;
    public bool isDead = false;

    [Header("Effects")]
    public AudioClip deathSound;
    public ParticleSystem deathParticles;

    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // No recibir daño si ya está muerto

        currentHealth -= damage;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        healthSlider.value = (float)currentHealth / maxHealth;
        healthText.text = $"{currentHealth}/{maxHealth}";
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        deathScreen.SetActive(true);
        Time.timeScale = 0f;

        // Desactivar componentes
        GetComponent<MovementPlayer>().enabled = false;
        GetComponent<PlayerCombat>().enabled = false;
    }

    public void Respawn()
    {
        currentHealth = maxHealth;
        isDead = false;
        deathScreen.SetActive(false);
        Time.timeScale = 1f;
        UpdateHealthUI();

        // Reactivar componentes
        GetComponent<MovementPlayer>().enabled = true;
        GetComponent<PlayerCombat>().enabled = true;

        // Volver a animación Idle
        animator.Play("Idle");
    }
}