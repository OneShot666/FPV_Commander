using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Info")]
    public string playerName = "Hero";

    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth = 50;

    [Header("Heal Test")]
    public int healPerSecond = 10;
    public bool isHealing = false;

    public void TakeDamage(float damage)
    {
        // Convert the damage value to an integer
        int dmgAmount = Mathf.CeilToInt(damage);
        currentHealth = Mathf.Max(0, currentHealth - dmgAmount);
        Debug.Log($"Player hurted: {currentHealth}/{maxHealth} (-{dmgAmount})");
    }

    public void Heal(float amount)
    {
        // Convert the healing value to an integer
        int healAmount = Mathf.CeilToInt(amount);
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        Debug.Log($"Player healed: {currentHealth}/{maxHealth} (+{healAmount})");
    }
}