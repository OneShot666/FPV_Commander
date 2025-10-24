using UnityEngine;

namespace Toggle {
    public class ParticleSystemComponent : BaseToggleComponent {
        [SerializeField] private ParticleSystem particlesSystem;

        protected override void ActivateComponent() {
            particlesSystem.Play();
        }

        protected override void DeactivateComponent() {
            particlesSystem.Stop();
        }
    }
}
