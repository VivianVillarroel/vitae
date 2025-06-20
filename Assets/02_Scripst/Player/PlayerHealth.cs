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
        animator.SetTrigger("Die");
        deathScreen.SetActive(true);
        Time.timeScale = 0f;

        if (deathSound) AudioSource.PlayClipAtPoint(deathSound, transform.position);
        if (deathParticles) Instantiate(deathParticles, transform.position, Quaternion.identity);
    }

    public void Respawn()
    {
        currentHealth = maxHealth;
        deathScreen.SetActive(false);
        Time.timeScale = 1f;
        animator.Play("Idle"); // Volver a la animación Idle
    }
}