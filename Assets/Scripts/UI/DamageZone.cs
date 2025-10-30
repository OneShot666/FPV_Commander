using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageZone : MonoBehaviour
{
    [Header("Healing Settings")] [SerializeField]
    private int damagePerSecond = 10;

    [SerializeField] private string playerTag = "Player";

    private PlayerStats playerStats;

    private void OnTriggerStay(Collider other)
    {
        PlayerStats ps = other.GetComponent<PlayerStats>();
        if (ps != null)
        {
            ps.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}