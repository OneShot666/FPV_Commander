using UnityEngine;

namespace Player {
    public class PlayerAnimation : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Transform leftArm;
        [SerializeField] private Transform rightArm;
        [SerializeField] private PlayerMovement playerMovement;

        [Header("Arm Swing Settings")]
        [SerializeField] private float swingSpeed = 2f;
        [SerializeField] private float swingMaxAngle = 15f;
        [SerializeField] private float sprintMultiplier = 1.8f;

        private float swingTimer;

        void Update() {
            if (!playerMovement) return;

            float moveSpeed = playerMovement.CurrentSpeed;                          // Get player's default speed
            bool isMoving = moveSpeed > 0.1f && playerMovement.IsGrounded;

            if (isMoving) {
                swingTimer += Time.deltaTime * swingSpeed * (moveSpeed / playerMovement.BaseSpeed);

                float swingAmount = swingMaxAngle * (moveSpeed / playerMovement.BaseSpeed);    // Get amplitude based on speed
                swingAmount = Mathf.Clamp(swingAmount, 0f, swingMaxAngle * sprintMultiplier);

                float leftRotation = Mathf.Sin(swingTimer) * swingAmount;           // Swing arms (opposed direction)
                float rightRotation = Mathf.Sin(swingTimer + Mathf.PI) * swingAmount;

                leftArm.localRotation = Quaternion.Euler(leftRotation, 0f, 0f);
                rightArm.localRotation = Quaternion.Euler(rightRotation, 0f, 0f);
            } else {                                                                // Slowly go back to neutral position
                leftArm.localRotation = Quaternion.Slerp(leftArm.localRotation, Quaternion.identity, Time.deltaTime * 5f);
                rightArm.localRotation = Quaternion.Slerp(rightArm.localRotation, Quaternion.identity, Time.deltaTime * 5f);
            }
        }
    }
}
