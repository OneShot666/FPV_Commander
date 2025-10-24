using UnityEngine.InputSystem;
using UnityEngine;

namespace Player {
    public class PlayerInteract : MonoBehaviour {
        public void OnInteract(InputAction.CallbackContext context) {
            if (context.performed && Camera.main) {
                Vector3 pos = Camera.main.transform.position;
                Vector3 forward = Camera.main.transform.forward;
                if (Physics.Raycast(pos, forward, out var hit, 3f)) {
                    Debug.DrawRay(pos, forward * 3f, Color.red, 1f);
                    Debug.Log(hit.transform.name);                              // Display target name
                }
            }
        }
    }
}
