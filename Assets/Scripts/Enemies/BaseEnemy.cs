using UnityEngine;

namespace Enemies {
    /// <summary> Main enemy behaviour : detect and pursue player </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class BaseEnemy : MonoBehaviour {
        [Header("Health settings")]
        [SerializeField] private float maxHealth = 100;

        [Header("Detection settings")]
        [Tooltip("Range of sight of enemy")]
        [SerializeField] private float detectionRange = 15f;
        [Tooltip("Angle of filed of vision (in degree)")]
        [SerializeField] private float fieldOfView = 120f;
        [Tooltip("Max height of detection")]
        [SerializeField] private float detectionHeight = 3f;

        [Header("Movement settings")]
        [Tooltip("Enemy move speed")]
        [SerializeField] private float moveSpeed = 3.5f;
        [Tooltip("Min distance to stop before reaching player (attack range")]
        [SerializeField] private float stoppingDistance = 1f;
        
        [Header("UI settings")]
        [SerializeField] private GameObject healthBarPrefab;

        [Header("Debug settings")]
        [SerializeField] private bool showGizmos = true;

        private Transform _player;
        private Rigidbody _rb;
        private HealthBar _healthBar;
        private bool _isPlayerDetected;
        private float _currentHealth;

        void Start() {
            var playerObj = GameObject.FindGameObjectWithTag("Player");         // Auto-find player
            if (playerObj) _player = playerObj.transform;

            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = true;
            _rb.isKinematic = false;
            
            _currentHealth = maxHealth;

            if (healthBarPrefab) {
                GameObject healthBarObj = Instantiate(healthBarPrefab, transform);
                _healthBar = healthBarObj.GetComponent<HealthBar>();
                _healthBar.Initialize(transform, maxHealth);
                _healthBar.SetHealth(_currentHealth);
                _healthBar.gameObject.SetActive(false);
            }
        }

        void FixedUpdate() {
            if (!_player) return;                                               // Player not found

            DetectPlayer();

            if (_isPlayerDetected) MoveTowardsPlayer();
            
            if (_currentHealth < maxHealth) DisplayHealthBar();
        }

        private void DetectPlayer() {
            _isPlayerDetected = false;

            Vector3 directionToPlayer = _player.position - transform.position;
            float distance = directionToPlayer.magnitude;

            if (distance > detectionRange) return;                              // Check distance
            if (Mathf.Abs(directionToPlayer.y) > detectionHeight) return;       // Check height

            float angle = Vector3.Angle(transform.forward, directionToPlayer);  // Check field of vision angle
            if (angle > fieldOfView / 2f) return;

            _isPlayerDetected = true;
        }

        private void MoveTowardsPlayer() {
            Vector3 dir = _player.position - transform.position;
            float distance = dir.magnitude;

            if (distance <= stoppingDistance) return;                           // If close enough from player

            dir.y = 0f;                                                         // Untouched : manage by gravity
            dir.Normalize();
            Quaternion targetRot = Quaternion.LookRotation(dir);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRot, Time.fixedDeltaTime * 5f));

            Vector3 move = dir * (moveSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(transform.position + move);
        }

        private void DisplayHealthBar() {
            if (_healthBar && !_healthBar.gameObject.activeSelf) _healthBar.gameObject.SetActive(true);
        }

        void OnDrawGizmosSelected() {
            if (!showGizmos) return;

            Gizmos.color = _isPlayerDetected ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward; // Field of vision
            Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward;

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, leftBoundary * detectionRange);
            Gizmos.DrawRay(transform.position, rightBoundary * detectionRange);
        }

        public void TakeDamage(float damage) {
            _currentHealth -= damage;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
            if (_healthBar) _healthBar.SetHealth(_currentHealth);
            if (_currentHealth <= 0) Die();
        }

        private void Die() {
            Destroy(gameObject, 1f);
        }
    }
}
