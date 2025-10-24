using UnityEngine;

namespace Toggle {
    public abstract class BaseToggleComponent : MonoBehaviour, IToggle {
        private bool isState;

        public virtual void Activate() {
            isState = true;
            ActivateComponent();
        }
        
        public virtual void Deactivate() {
            isState = false;
            DeactivateComponent();
        }

        protected abstract void ActivateComponent();

        protected abstract void DeactivateComponent();

        public virtual void Switch() {
            if (isState) Activate();
            else Deactivate();
        }
    }
}
