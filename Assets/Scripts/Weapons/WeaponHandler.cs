using JetBrains.Annotations;
using UnityEngine.UI;
using UnityEngine;
using Player;

namespace Weapons {
    public class WeaponHandler : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Transform arm;                                 // Player's arm
        [SerializeField, CanBeNull] private Transform hand;                     // Player's hand
        [SerializeField, CanBeNull] private Transform socket;                   // Player's weapon socket
        [SerializeField] private Weapon weaponPrefab;                           // Equipped weapon
        [SerializeField, CanBeNull] private GameObject crosshairPrefab;         // Sight image for UI
        [SerializeField, CanBeNull] private Canvas canvasUI;                    // UI object
        [SerializeField, CanBeNull] private Transform weaponsParent;            // To put weapons in order
        [SerializeField] private PlayerInputHandler inputHandler;               // InputSystem Script
        [SerializeField] private PlayerAnimation playerAnimation;               // Player Animation Script
        [SerializeField] private Text pickupText;                               // Text to display when can pick up

        [Header("Settings")]
        [SerializeField, Range(0, 10)] private float collectRange = 8f;
        [SerializeField, Range(0.5f, 1f)] private float collectRadius = 1f;

        private Camera mainCamera;
        private GameObject _crosshairInstance;
        private Weapon _currentWeapon;                                          // Weapon in hand
        private Weapon _socketWeapon;                                           // Weapon in socket (unequipped) 
        private bool _isEquipped;

        void Start() {
            mainCamera = Camera.main;
            if (!weaponsParent) {
                GameObject check = GameObject.Find("Dropped weapons");
                weaponsParent = check ? check.transform : new GameObject("Dropped weapons").transform;
            }

            CollectWeapon(weaponPrefab);

            if (arm && !hand) hand = arm.GetChild(0);                           // Get first children

            if (!canvasUI) canvasUI = FindAnyObjectByType<Canvas>();

            if (crosshairPrefab && canvasUI) {                                  // Create and hide crosshair
                _crosshairInstance = Instantiate(crosshairPrefab, canvasUI.transform);
                _crosshairInstance.SetActive(false);                            // Hide by default (weapon is unequipped)
                RectTransform rect = _crosshairInstance.GetComponent<RectTransform>();
                rect.anchoredPosition = Vector2.zero;                           // Center of screen
                rect.localScale = crosshairPrefab.transform.localScale;
            }

            if (pickupText) pickupText.gameObject.SetActive(false);             // Invisible by default
        }

        void Update() {
            HandleEquipInput();
            HandleFireInput();
            HandleDropInput();
            TryPickUpWeapon();
        }

        private void CollectWeapon([CanBeNull] Weapon weapon) {                 // Create and position weapon
            if (!weapon || !hand) return;
            
            if (_currentWeapon) DropWeapon();                                   // Drop current weapon if has one

            GameObject instance = Instantiate(weapon.gameObject, hand);         // Add weapon in player's hand
            instance.name = weapon.name;
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);   // Rotate on correct axis
            instance.transform.localScale = Vector3.one;
            Rigidbody rb = instance.GetComponent<Rigidbody>();
            if (rb) Destroy(rb);                                                // Don't affect equipped weapon by physic
            Collider col = instance.GetComponent<Collider>();
            if (col) col.enabled = false;
            _currentWeapon = instance.GetComponent<Weapon>();
            _currentWeapon.gameObject.SetActive(_isEquipped);
            
            if (!socket) return;

            instance = Instantiate(weapon.gameObject, socket);                  // Add weapon in player's socket
            instance.name = weapon.name;
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);   // Rotate on correct axis
            instance.transform.localScale = Vector3.one;
            rb = instance.GetComponent<Rigidbody>();
            if (rb) Destroy(rb);                                                // Don't affect equipped weapon by physics
            col = instance.GetComponent<Collider>();
            if (col) col.enabled = false;
            _socketWeapon = instance.GetComponent<Weapon>();
            _socketWeapon.gameObject.SetActive(!_isEquipped);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void DropWeapon() {
            if (!_currentWeapon) return;                                        // No weapon to drop

            GameObject dropped = Instantiate(_currentWeapon.gameObject, weaponsParent); // Add copy to main scene
            dropped.name = _currentWeapon.name;
            dropped.transform.position = _currentWeapon.transform.position + transform.forward * 0.5f + transform.up * 0.5f;
            Rigidbody rb = dropped.GetComponent<Rigidbody>();
            if (!rb) rb = dropped.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;                                               // Apply physics to weapon
            if (dropped.TryGetComponent(out Collider col)) col.enabled = true;
            dropped.gameObject.SetActive(true);                                 // Display object

            UnequipWeapon();

            Destroy(_currentWeapon.gameObject);
            if (_socketWeapon) Destroy(_socketWeapon.gameObject);
            _currentWeapon = null;                                              // ? Sure about that (should create a new instance ?)
            _socketWeapon = null;
        }

        private void EquipWeapon() {
            if (!_currentWeapon) return;

            _isEquipped = true;
            _currentWeapon.gameObject.SetActive(_isEquipped);                   // Display weapon in hand
            if (_crosshairInstance) _crosshairInstance.SetActive(_isEquipped);  // Display crosshair
            if (socket) _socketWeapon.gameObject.SetActive(!_isEquipped);       // Display weapon in socket
            if (playerAnimation) playerAnimation.SetIsArmed(_isEquipped);       // Update animation script
        }

        private void UnequipWeapon() {
            if (!_currentWeapon) return;

            _isEquipped = false;
            _currentWeapon.gameObject.SetActive(_isEquipped);
            if (_crosshairInstance) _crosshairInstance.SetActive(_isEquipped);
            if (socket) _socketWeapon.gameObject.SetActive(!_isEquipped);
            if (playerAnimation) playerAnimation.SetIsArmed(_isEquipped);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void TryPickUpWeapon() {                                        // If weapon in sight
            if (!mainCamera) return;

            if (pickupText) pickupText.gameObject.SetActive(false);

            Vector3 point1 = transform.position + Vector3.up * 0.5f;
            Vector3 point2 = transform.position + Vector3.up * 1.5f;
            if (Physics.CapsuleCast(point1, point2, collectRadius, mainCamera.transform.forward, 
                    out RaycastHit hit, collectRange)) {
                Weapon weapon = hit.collider.GetComponentInParent<Weapon>();
                if (weapon && weapon != _currentWeapon) {
                    if (pickupText) {                                           // Show text
                        pickupText.text = $"Pick up '{weapon.name}' (E)";
                        pickupText.gameObject.SetActive(true);
                    }
                    HandleCollectInput(weapon);
                }
            }
        }

        private void PickUpWeapon(Weapon weapon) {
            CollectWeapon(weapon);
            EquipWeapon();
            Destroy(weapon.gameObject);                                         // Remove weapon on the ground
            if (pickupText) pickupText.gameObject.SetActive(false);
        }

        private void HandleEquipInput() {
            if (inputHandler.EquipInput) {
                inputHandler.ResetEquipInput();                                 // Avoid double activation

                _isEquipped = !_isEquipped;
                if (_isEquipped) EquipWeapon();
                else UnequipWeapon();
            }
        }

        private void HandleFireInput() {
            if (_isEquipped && inputHandler.FireInput) {
                inputHandler.ResetFireInput();
                _currentWeapon.Fire();
            }
        }

        private void HandleDropInput() {
            if (inputHandler.DropInput) {
                //inputHandler.ResetDropInput(); !!
                DropWeapon();
            }
        }

        private void HandleCollectInput(Weapon weapon) {
            if (inputHandler.CollectInput) {
                inputHandler.ResetCollectInput();
                PickUpWeapon(weapon);
            }
        }
    }
}
