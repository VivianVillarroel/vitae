using UnityEngine;
using UnityEngine.UI;
using System;

public class DayNightCycle : MonoBehaviour
{
    public Light sun;
    public float secondsInFullDay = 120f; // 2 minutos para un ciclo completo
    [Range(0, 1)] public float currentTimeOfDay = 0.5f;
    public float timeMultiplier = 1f;

    public Text timeText;
    public Material daySkybox;
    public Material nightSkybox;
    public Material rainSkybox;

    private float sunInitialIntensity;
    public bool isNight = false;
    public WeatherSystem weatherSystem;

    void Start()
    {
        sunInitialIntensity = sun.intensity;
        RenderSettings.skybox = daySkybox;
    }

    void Update()
    {
        UpdateSun();
        UpdateTime();
        UpdateSkybox();

        // Mostrar hora en formato 24h
        float displayTime = currentTimeOfDay * 24f;
        timeText.text = string.Format("{0:00}:{1:00}", (int)displayTime, (int)((displayTime % 1) * 60));
    }

    void UpdateSun()
    {
        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 170, 0);

        float intensityMultiplier = 1;
        if (currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.75f)
        {
            intensityMultiplier = 0;
            isNight = true;
        }
        else if (currentTimeOfDay <= 0.25f)
        {
            intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f));
            isNight = false;
        }
        else if (currentTimeOfDay >= 0.73f)
        {
            intensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.73f) * (1 / 0.02f)));
            isNight = false;
        }
        else
        {
            isNight = false;
        }

        sun.intensity = sunInitialIntensity * intensityMultiplier;
    }

    void UpdateTime()
    {
        currentTimeOfDay += (Time.deltaTime / secondsInFullDay) * timeMultiplier;

        if (currentTimeOfDay >= 1)
        {
            currentTimeOfDay = 0;
        }
    }

    void UpdateSkybox()
    {
        if (weatherSystem != null && weatherSystem.isRaining)
        {
            RenderSettings.skybox = rainSkybox;
        }
        else if (isNight)
        {
            RenderSettings.skybox = nightSkybox;
        }
        else
        {
            RenderSettings.skybox = daySkybox;
        }
    }

    public bool IsNightTime()
    {
        return isNight;
    }
}