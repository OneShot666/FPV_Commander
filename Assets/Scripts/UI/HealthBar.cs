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

        healthSlider.maxValue = playerStats.maxHealth;
        healthSlider.value = playerStats.currentHealth;
        healthText.text = $"{playerStats.currentHealth}/{playerStats.maxHealth}";
    }

    private void Update()
    {
        if (playerStats == null) return;

        healthSlider.value = playerStats.currentHealth;
        healthText.text = $"{playerStats.currentHealth}/{playerStats.maxHealth}";
    }
}