using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    public Material sunnySkybox;
    public Material cloudySkybox;


    public ParticleSystem rainParticles;
    public AudioSource rainSound;
    public Light directionalLight;
    public bool isRaining = false;

    [Header("Rain Settings")]
    public Color rainFogColor = new Color(0.3f, 0.3f, 0.3f);
    public float rainFogDensity = 0.03f;

    private Color defaultFogColor;
    private float defaultFogDensity;

    void Start()
    {
        defaultFogColor = RenderSettings.fogColor;
        defaultFogDensity = RenderSettings.fogDensity;
        ToggleRain(false);
    }

    public void ToggleRain(bool state)
    {
        isRaining = state;
        if (state)
        {
            rainParticles.Play();
            rainSound.Play();
            RenderSettings.fog = true;
            RenderSettings.fogColor = rainFogColor;
            RenderSettings.fogDensity = rainFogDensity;
        }
        else
        {
            rainParticles.Stop();
            rainSound.Stop();
            RenderSettings.fog = false;
        }
        if (state)
        {
            // Lluvia ON
            rainParticles.Play();
            rainSound.Play();
            RenderSettings.fog = true;
            RenderSettings.fogColor = rainFogColor;
            RenderSettings.fogDensity = rainFogDensity;
            RenderSettings.skybox = cloudySkybox; // 💨 cielo nublado
        }
        else
        {
            // Lluvia OFF
            rainParticles.Stop();
            rainSound.Stop();
            RenderSettings.fog = false;
            RenderSettings.skybox = sunnySkybox; // ☀ cielo soleado
        }


    }

    void Update()
    {
        // Solo para pruebas: T para alternar clima
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleRain(!isRaining);
        }
    }
}
