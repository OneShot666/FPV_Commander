using UnityEngine;

namespace Toggle {
    public abstract class BaseToggleSetter : IToggle {
        [SerializeField] private BaseToggleComponent toggleComponent;
        
        public void Activate() {
            toggleComponent.Activate();
        }

        public void Deactivate() {
            toggleComponent.Deactivate();
        }

        public void Switch() {
            toggleComponent.Switch();
        }
    }
}
