using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HealingZone : MonoBehaviour
{
    [Header("Healing Settings")]
    [SerializeField] private int healPerSecond = 10;
    [SerializeField] private string playerTag = "Player";

    private PlayerStats playerStats;

    private void OnTriggerStay(Collider other)
    {
        // Check if the object has a PlayerStats component
        PlayerStats ps = other.GetComponent<PlayerStats>();
        if (ps != null)
        {
            ps.Heal(healPerSecond * Time.deltaTime);
        }
    }
}