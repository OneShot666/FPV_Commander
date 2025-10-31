using UnityEngine;
using UnityEngine.UI;
using TMPro;            // Implement TextMeshProUI components to update the healthText

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    [Header("UI Elements")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    private void Start()
    {
        if (playerStats == null)
        {
            Debug.LogError("[HealthBar] PlayerStats non assigné !");
            enabled = false;
            return;
        }

        // Set the initial values of the health bar at the start
        healthSlider.maxValue = playerStats.maxHealth;
        healthSlider.value = playerStats.currentHealth;
        healthText.text = $"{playerStats.currentHealth}/{playerStats.maxHealth}";
    }

    private void Update()
    {
        if (playerStats == null) return;

        // Update the slider and text every frame to match the player's current health
        healthSlider.value = playerStats.currentHealth;
        healthText.text = $"{playerStats.currentHealth}/{playerStats.maxHealth}";
    }
}