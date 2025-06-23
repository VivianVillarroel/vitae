using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public float healthRegenRate = 1f;
    public float healthRegenDelay = 5f;
    private float lastDamageTime;

    [Header("Energy")]
    public int maxEnergy = 100;
    public int currentEnergy;
    public Slider energySlider;
    public float energyDrainRate = 5f;
    public float energyRegenRate = 10f;

    [Header("UI")]
    public Text healthText;
    public Text energyText;
    public GameObject deathScreen;

    void Start()
    {
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
        UpdateUI();
    }

    void Update()
    {
        // Regeneración de salud
        if (Time.time > lastDamageTime + healthRegenDelay && currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + (int)(healthRegenRate * Time.deltaTime));
            UpdateUI();
        }

        // Regeneración de energía
        if (currentEnergy < maxEnergy)
        {
            currentEnergy = Mathf.Min(maxEnergy, currentEnergy + (int)(energyRegenRate * Time.deltaTime));
            UpdateUI();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        lastDamageTime = Time.time;
        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool UseEnergy(int amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    void Die()
    {
        deathScreen.SetActive(true);
        Time.timeScale = 0f;
        // Aquí puedes agregar lógica de respawn o game over
    }

    void UpdateUI()
    {
        healthSlider.value = (float)currentHealth / maxHealth;
        energySlider.value = (float)currentEnergy / maxEnergy;
        healthText.text = $"{currentHealth}/{maxHealth}";
        energyText.text = $"{currentEnergy}/{maxEnergy}";
    }

    public void Respawn()
    {
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
        deathScreen.SetActive(false);
        Time.timeScale = 1f;
        // Agrega lógica de posición de respawn aquí
    }
}