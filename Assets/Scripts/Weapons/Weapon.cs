using JetBrains.Annotations;
using UnityEngine;
using Player;

// L After in enemy spawner, enemies can take damage if touched by bullets -> use damage variable
namespace Weapons {
    public class Weapon : MonoBehaviour {
        [Header("References")]
        [SerializeField, CanBeNull] private PlayerMovement playerMovement;
        [SerializeField, CanBeNull] private Transform bulletsParent;            // To put bullets in order

        [Header("Settings")]
        [SerializeField] private float damage = 10f;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;                           // Weapon's muzzle position
        [SerializeField] private float bulletOffset = 0.2f;
        [SerializeField] private float recoilDistance = 0.1f;
        [SerializeField] private float recoilDuration = 0.05f;

        private Vector3 _playerVelocity;
        private Vector3 _lastPosition;
        private Vector3 _initLocalPosition;
        private Vector3 _recoilTargetPosition;

        void Start() {
            _initLocalPosition = transform.localPosition;
            _recoilTargetPosition = _initLocalPosition;

            if (!playerMovement) playerMovement = GetComponentInParent<PlayerMovement>();
            if (!bulletsParent) {
                GameObject check = GameObject.Find("Bullets");
                bulletsParent = check ? check.transform : new GameObject("Bullets").transform;
            }
        }

        void Update() {
            GetPlayerVelocity();

            transform.localPosition = Vector3.Lerp(transform.localPosition, _initLocalPosition, Time.deltaTime / recoilDuration);
            
            if (!_recoilTargetPosition.Equals(_initLocalPosition)) _recoilTargetPosition = _initLocalPosition;
        }

        private void GetPlayerVelocity() {
            if (!playerMovement) return;

            Vector3 position = playerMovement.transform.position;
            _playerVelocity = (position - _lastPosition) / Time.deltaTime;
            _lastPosition = position;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void Fire() {
            if (!bulletPrefab || !firePoint) return;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position + firePoint.up * bulletOffset, 
                firePoint.rotation, bulletsParent);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.SetDamage(damage);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb) rb.linearVelocity += _playerVelocity;                       // Add player velocity to bullet velocity
            
            ApplyRecoil();
        }

        private void ApplyRecoil() {
            if (recoilDuration <= 0) return;

            _recoilTargetPosition = _initLocalPosition - transform.forward * recoilDistance;
        }
    }
}
