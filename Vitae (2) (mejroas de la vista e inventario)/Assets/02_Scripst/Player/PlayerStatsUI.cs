using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    public Text speedText;
    public MovementPlayer playerMovement;
    public WeatherSystem weatherSystem;

    void Update()
    {
        float speedPercentage = (playerMovement.speedMovement / playerMovement.baseSpeed) * 100f;
        string weatherEffect = weatherSystem.isRaining ? " (Lluvia -40%)" : "";

        speedText.text = $"Velocidad: {speedPercentage:F0}%{weatherEffect}";
    }
}