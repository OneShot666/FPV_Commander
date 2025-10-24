using UnityEngine;
using Player;

// . Weapon isn't at the right scale, position and rotation in player's arm
// ! Crosshair isn't display on screen (exists on scene)
// ! Arm don't raise enough
// ! Bullets stays at start position
// ? Move arm animation in PlayerAnimation.cs
// L Add weaponSocket -> where the weapon is when unequipped
namespace Weapons {
    public class WeaponHandler : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Transform rightArm;                            // Player's right arm
        [SerializeField] private GameObject crosshairPrefab;                    // Sight image for UI
        [SerializeField] private PlayerInputHandler inputHandler;               // InputSystem Script
        [SerializeField] private Weapon weaponPrefab;

        [Header("Arm Movement")]
        [SerializeField] private Vector3 equippedArmRotation = new(-90f, 0f, 0f);
        [SerializeField] private float armLiftSpeed = 5f;
        [SerializeField] private float armSwayAmplitude = 0.5f;
        [SerializeField] private float armSwaySpeed = 5f;

        private GameObject _crosshairInstance;
        private Quaternion _initialArmRotation;
        private Weapon equippedWeapon;
        private bool _isEquipped;

        void Start() {
            SpawnWeapon();

            if (rightArm) _initialArmRotation = rightArm.localRotation;
            
            if (crosshairPrefab) {                                              // Create and hide crosshair
                _crosshairInstance = Instantiate(crosshairPrefab);
                _crosshairInstance.SetActive(false);                            // Hide by default (weapon is unequipped)
            }
        }

        void Update() {
            HandleEquipInput();
            HandleFireInput();
            HandleArmMovement();
        }

        private void SpawnWeapon() {                                            // Create and position weapon
            if (!weaponPrefab || !rightArm) return;

            GameObject instance = Instantiate(weaponPrefab.gameObject, rightArm);

            // ... Work in progress
            instance.transform.position = rightArm.position + rightArm.forward * 0.1f + rightArm.up * -0.1f; // ajuste la distance
            instance.transform.rotation = rightArm.rotation * Quaternion.Euler(90f, 0f, 0f); // rotation de 90° sur X
            instance.transform.localScale = weaponPrefab.transform.localScale;    // Keep prefab scale
            instance.SetActive(_isEquipped);

            equippedWeapon = instance.GetComponent<Weapon>();
        }

        private void HandleEquipInput() {
            if (inputHandler.EquipInput) {
                _isEquipped = !_isEquipped;
                inputHandler.ResetEquipInput();                                 // Avoid double activation

                if (_crosshairInstance) _crosshairInstance.SetActive(_isEquipped);

                if (equippedWeapon) equippedWeapon.gameObject.SetActive(_isEquipped);
            }
        }

        private void HandleFireInput() {
            if (_isEquipped && inputHandler.FireInput) {
                equippedWeapon?.Fire();
                inputHandler.ResetFireInput();
            }
        }

        private void HandleArmMovement() {
            if (!rightArm) return;

            if (_isEquipped) {
                Quaternion targetRotation = Quaternion.Euler(equippedArmRotation);  // Raise arm
                rightArm.localRotation = Quaternion.Slerp(rightArm.localRotation, targetRotation, Time.deltaTime * armLiftSpeed);

                float sway = Mathf.Sin(Time.time * armSwaySpeed) * armSwayAmplitude;
                rightArm.localRotation *= Quaternion.Euler(sway, 0f, 0f);       // Small pitch when player is moving
            } else {                                                            // Slowly go back to neutral position
                rightArm.localRotation = Quaternion.Slerp(rightArm.localRotation, _initialArmRotation, Time.deltaTime * armLiftSpeed);
            }
        }
    }
}
