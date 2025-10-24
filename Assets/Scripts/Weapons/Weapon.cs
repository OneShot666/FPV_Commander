using UnityEngine;

namespace Weapons {
    public class Weapon : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;                           // Weapon's muzzle position

        // ReSharper disable Unity.PerformanceAnalysis
        public void Fire() {
            if (!bulletPrefab || !firePoint) return;
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}
