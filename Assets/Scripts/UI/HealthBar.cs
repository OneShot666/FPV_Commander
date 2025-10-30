using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        // Initialize the health bar values at the start
        healthSlider.maxValue = playerStats.maxHealth;
        healthSlider.value = playerStats.currentHealth;
        healthText.text = $"{playerStats.currentHealth}/{playerStats.maxHealth}";
    }

    private void Update()
    {
        if (playerStats == null) return;

        // Update the slider value and health text in real time
        healthSlider.value = playerStats.currentHealth;
        healthText.text = $"{playerStats.currentHealth}/{playerStats.maxHealth}";
    }
}