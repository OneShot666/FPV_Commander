using Toggle;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    public class PlayerInteract : MonoBehaviour {
        [SerializeField] private float interactDistance = 3f;
        public void OnInteract(InputAction.CallbackContext context) {
            if (context.performed && Camera.main) {
                Vector3 pos = Camera.main.transform.position;
                Vector3 forward = Camera.main.transform.forward;

                if (Physics.Raycast(pos, forward, out var hit, interactDistance)) {
                    Debug.DrawRay(pos, forward * interactDistance, Color.red, 1f);
                    Debug.Log("Cible détectée : " + hit.transform.name);

                    var toggle = hit.transform.GetComponent<BaseToggleComponent>();
                    if (toggle != null) {
                        toggle.Switch();
                    }
                    var interactable = hit.transform.GetComponent<BaseInteractableItem>();
                    if (interactable != null) {
                        interactable.Switch();
                        return;
                    }
                }
            }
        }
    }
}