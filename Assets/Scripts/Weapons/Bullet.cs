using UnityEngine;

namespace Weapons {
    [RequireComponent(typeof(Rigidbody))]                                       // To detect other objects
    public class Bullet : MonoBehaviour {
        [SerializeField] private float lifeTime = 5f;
        [SerializeField] private float maxDistance = 100f;
        [SerializeField] private float speed = 50f;

        private Vector3 _startPosition;
        private Rigidbody _rb;

        void Start() {
            _rb = GetComponent<Rigidbody>();
            // _rb.useGravity = false;                                             // [Optional] For no bullet drop
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;     // [Optional] Faster bullets
            _rb.linearVelocity = transform.forward * speed;                     // Moves along forward direction

            _startPosition = transform.position;
            Destroy(gameObject, lifeTime);                                      // Die after its lifetime
        }

        void Update() {                                                         // Delete self if too far from start position
            if (Vector3.Distance(_startPosition, transform.position) > maxDistance) Destroy(gameObject);
        }

        void OnCollisionEnter() {
            Destroy(gameObject);                                                // Delete self if hit anything
        }
    }
}
