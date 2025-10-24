using UnityEngine;

namespace Toggle {
    public abstract class TargetToggleComponent : BaseToggleComponent {
        [SerializeField] private GameObject target;

        private void Reset() {
            target = gameObject;
        }

        protected override void ActivateComponent() {
            target.SetActive(true);
        }

        protected override void DeactivateComponent() {
            target.SetActive(false);
        }
    }
}
