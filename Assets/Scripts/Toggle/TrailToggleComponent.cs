using JetBrains.Annotations;
using UnityEngine;

namespace Toggle {
    public class TrailToggleComponent : BaseToggleComponent {
        [SerializeField, CanBeNull] private TrailRenderer trail;

        protected override void ActivateComponent() {
            if (trail) trail.emitting = true;
        }

        protected override void DeactivateComponent() {
            if (trail) trail.emitting = false;
        }
    }
}
