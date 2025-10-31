using UnityEngine;
using Enemies;

namespace Weapons {
    [RequireComponent(typeof(Rigidbody))]                                       // To detect other objects
    public class Bullet : MonoBehaviour {
        [Header("Bullet Properties")]
        [SerializeField] private float speed = 20f;
        [Tooltip("If goes beyond these distance, delete itself")]
        [SerializeField] private float maxDistance = 100f;
        [SerializeField] private GameObject bulletImpact;

        [Header("Optional")]
        [SerializeField] private bool isBulletDrop;
        [Tooltip("Stay 5 seconds on objects hit before disappearing")]
        [SerializeField] private bool isStay;
        [Tooltip("[Later] Can penetrate object or touch enemies several time")]
        [SerializeField] private bool isPenetrating;

        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private Rigidbody _rb;
        private bool _hasHit;                                                   // Only touch once
        private float _damage;

        void Start() {
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = isBulletDrop;
            if (isPenetrating) _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;   // Faster bullets
            _rb.linearVelocity = transform.up * speed;                          // Moves along forward direction

            Collider bulletCollider = GetComponent<Collider>();
            Collider playerCollider = GameObject.FindWithTag("Player").GetComponent<Collider>();
            Physics.IgnoreCollision(bulletCollider, playerCollider);            // Don't collide with player

            _startRotation = transform.rotation;
            _startPosition = transform.position;
        }

        void Update() {                                                         // Delete self if too far from start position
            if (Vector3.Distance(_startPosition, transform.position) > maxDistance) Destroy(gameObject);
        }

        private void FixedUpdate() {
            if (isBulletDrop && _rb.linearVelocity.sqrMagnitude > 0.01f) {      // Look where it's going
                Quaternion look = Quaternion.LookRotation(_rb.linearVelocity.normalized, Vector3.up);
                transform.rotation = look * Quaternion.Euler(90f, 0f, 0f);
            }
        }

        void OnCollisionEnter(Collision collision) {
            if (_hasHit) return;
            _hasHit = true;

            ApplyDamage(collision);

            if (bulletImpact) ApplyBulletImpact(collision);

            if (isStay) StickToSurface(collision);
            else Destroy(gameObject);                                           // Delete self if hit anything
        }

        public void SetDamage(float amount) => _damage = amount;

        private void ApplyDamage(Collision collision) {
            BaseEnemy enemyScript = collision.collider.GetComponent<BaseEnemy>();
            if (!enemyScript) enemyScript = collision.collider.GetComponentInParent<BaseEnemy>();
            if (enemyScript) enemyScript.TakeDamage(_damage);
        }

        private void ApplyBulletImpact(Collision collision) {                   // Add impact on scene
            ContactPoint contact = collision.contacts[0];
            Quaternion rotation = Quaternion.LookRotation(-contact.normal);
            Vector3 position = contact.point + contact.normal * 0.001f;         // Slightly offset : avoid z-axis pb

            GameObject impact = Instantiate(bulletImpact, position, rotation);
            impact.name = bulletImpact.name;
            impact.transform.localScale = bulletImpact.transform.localScale;
            impact.transform.Rotate(Vector3.right * -90f);                      // Adjust rotation
            Transform target = contact.otherCollider.transform;
            if (target) impact.transform.SetParent(target, true);

            Destroy(impact, 10f);                                               // Remove after a moment
        }

        private void StickToSurface(Collision collision) {
            _startRotation = transform.rotation;

            _rb.isKinematic = true;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;

            ContactPoint contact = collision.contacts[0];
            transform.position = contact.point + contact.normal * 0.001f;
            transform.rotation = _startRotation;

            transform.SetParent(collision.transform, true);
            Destroy(gameObject, 5f);                                            // Remove after a moment
        }
    }
}
