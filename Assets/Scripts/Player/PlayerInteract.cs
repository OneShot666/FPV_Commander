using Toggle;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    public class PlayerInteract : MonoBehaviour {
        [SerializeField] private float interactDistance = 3f;
        
        // Called when the "Interact" input action is triggered
        public void OnInteract(InputAction.CallbackContext context) {
            if (context.performed && Camera.main) {
                Vector3 pos = Camera.main.transform.position;
                Vector3 forward = Camera.main.transform.forward;
                
                // Cast a ray to detect objects
                if (Physics.Raycast(pos, forward, out var hit, interactDistance)) {
                    Debug.DrawRay(pos, forward * interactDistance, Color.red, 1f);
                    Debug.Log("Cible détectée : " + hit.transform.name);
                    
                    // Try to find a Toggle component and call its Switch() method
                    var toggle = hit.transform.GetComponentInChildren<BaseToggleComponent>();
                    if (toggle != null) {
                        toggle.Switch();
                    }
                    // Try to find an Interactable component and call its Switch() method
                    var interactable = hit.transform.GetComponentInParent<BaseInteractableItem>();
                    if (interactable != null) {
                        interactable.Switch();
                        return;
                    }
                }
            }
        }
    }
}