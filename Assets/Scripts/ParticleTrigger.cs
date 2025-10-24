using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ParticleTrigger : MonoBehaviour {
    [SerializeField] private ParticleSystem particlesSystem;

    private void Start() {
        Collider col = GetComponent<Collider>();                                // Make button a trigger
        col.isTrigger = true;

        if (particlesSystem != null) particlesSystem.Stop();                    // Disable particles at first
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && particlesSystem != null) particlesSystem.Play();
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player") && particlesSystem != null) particlesSystem.Stop();
    }
}
