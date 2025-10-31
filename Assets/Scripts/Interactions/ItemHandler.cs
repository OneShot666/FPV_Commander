using JetBrains.Annotations; // Allow to use the nullability attributes [CanBeNull] of Rider
using UnityEngine;
using Player;               // Used to access player related scripts
using TMPro;                // Implement TextMeshProUI components to indicate the actions in front of items
using Weapons;             // Used to access weapon related scripts

namespace Interactions {
    public class ItemHandler : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Transform arm;                                 // Player's arm
        [SerializeField, CanBeNull] private Transform hand;                     // Player's hand
        [SerializeField, CanBeNull] private Transform socket;                   // Player's item socket
        [SerializeField] private BaseInteractableItem itemPrefab;               // Equipped item
        [SerializeField] private GameObject bulletImpactPrefab;
        [SerializeField, CanBeNull] private Canvas canvasUI;                    // UI object
        [SerializeField, CanBeNull] private Transform itemsParent;              // To put items in order
        [SerializeField] private PlayerInputHandler inputHandler;               // InputSystem Script
        [SerializeField] private PlayerAnimation playerAnimation;               // Player Animation Script
        [SerializeField] private TextMeshProUGUI pickupText;                    // Text to display when can pick up

        [Header("Settings")]
        [SerializeField, Range(0, 10)] private float collectRange = 8f;
        [SerializeField, Range(0.5f, 1f)] private float collectRadius = 1f;

        private Camera mainCamera;
        private BaseInteractableItem _currentItem;                                          // Item in hand
        private BaseInteractableItem _socketItem;                                           // Item in socket (unequipped) 
        private bool _isEquipped;

        void Start() {
            mainCamera = Camera.main;
            
            if (!itemsParent) {
                GameObject check = GameObject.Find("Dropped items");
                itemsParent = check ? check.transform : new GameObject("Dropped items").transform;
            }

            CollectItem(itemPrefab);

            if (arm && !hand) hand = arm.GetChild(0);                           // Get first children

            if (!canvasUI) canvasUI = FindAnyObjectByType<Canvas>();
            
            if (pickupText) pickupText.gameObject.SetActive(false);             // Invisible by default
        }

        void Update() {
            HandleEquipInput();
            HandleActivateInput();
            HandleDropInput();
            TryPickUpItem();
        }

        // Destroy Bullet Impact and bullets from item
        private BaseInteractableItem CleanItem(BaseInteractableItem item)
        {
            foreach (Transform impact in item.GetComponentsInChildren<Transform>(true))
            {
                if (impact == item.transform) continue;
                if (impact.name == bulletImpactPrefab.name || impact.GetComponent<Bullet>())
                {
                    print($"{impact.name} Destroyed");
                    Destroy(impact.gameObject);
                }
            }
            
            return item;
        }

        private void CollectItem([CanBeNull] BaseInteractableItem item) {        // Create and position item
            if (!item || !hand) return;
            
            if (_currentItem) DropItem();                                       // Drop current item if has one
            item = CleanItem(item);
            print($"socket_instance {item.name} has been equipped");
            // TODO: if shot item is equipped in hand but not in socket, it isn't create ever after

            GameObject instance = Instantiate(item.gameObject, hand);           // Add item in player's hand
            instance.name = item.name;
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);   // Rotate on correct axis
            instance.transform.localScale = Vector3.one;
            Rigidbody rb = instance.GetComponent<Rigidbody>();
            if (rb) Destroy(rb);                                                // Don't affect equipped item by physic
            Collider col = instance.GetComponent<Collider>();
            if (col) col.enabled = false;
            _currentItem = instance.GetComponent<BaseInteractableItem>();
            _currentItem = CleanItem(_currentItem);
            _currentItem.gameObject.SetActive(_isEquipped);
            _currentItem.SetMaterials(item.originalMaterials);
            _currentItem.Equip();
            
            if (!socket) return;

            GameObject socket_instance = Instantiate(item.gameObject, socket);                  // Add item in player's socket
            socket_instance.name = item.name;
            socket_instance.transform.localPosition = Vector3.zero;
            socket_instance.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);           // Rotate on correct axis
            socket_instance.transform.localScale = Vector3.one;
            rb = socket_instance.GetComponent<Rigidbody>();
            if (rb) Destroy(rb);                                                               // Don't affect equipped item by physics
            col = socket_instance.GetComponent<Collider>();
            if (col) col.enabled = false;
            _socketItem = socket_instance.GetComponent<BaseInteractableItem>();
            _socketItem = CleanItem(_socketItem);
            _socketItem.gameObject.SetActive(!_isEquipped);
            _socketItem.SetMaterials(item.originalMaterials);
            _socketItem.Equip();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void DropItem() {
            if (!_currentItem) return;                                              // No item to drop

            GameObject dropped = Instantiate(_currentItem.gameObject, itemsParent); // Add copy to main scene
            dropped.name = _currentItem.name;
            dropped.transform.position = _currentItem.transform.position + transform.forward * 0.5f + transform.up * 0.5f;
            Rigidbody rb = dropped.GetComponent<Rigidbody>();
            if (!rb) rb = dropped.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;                                                   // Apply physics to item
            if (dropped.TryGetComponent(out Collider col)) col.enabled = true;
            dropped.gameObject.SetActive(true);                                     // Display object
            BaseInteractableItem dropItem = dropped.GetComponent<BaseInteractableItem>();
            dropItem.isEquipped = false;

            UnequipItem();

            Destroy(_currentItem.gameObject);
            if (_socketItem) Destroy(_socketItem.gameObject);
            _currentItem = null;                                              // ? Sure about that (should create a new instance ?)
            _socketItem = null;
        }

        private void EquipItem() {
            if (!_currentItem) return;

            _isEquipped = true;
            _currentItem.gameObject.SetActive(_isEquipped);                   // Display item in hand
            if (socket) _socketItem.gameObject.SetActive(!_isEquipped);       // Display item in socket
        }

        private void UnequipItem() {
            if (!_currentItem) return;

            _isEquipped = false;
            _currentItem.gameObject.SetActive(_isEquipped);
            if (socket) _socketItem.gameObject.SetActive(!_isEquipped);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void TryPickUpItem() {                                        // If item in sight
            if (!mainCamera) return;

            if (pickupText) pickupText.gameObject.SetActive(false);

            Vector3 point1 = transform.position + Vector3.up * 0.5f;
            Vector3 point2 = transform.position + Vector3.up * 1.5f;
            if (Physics.CapsuleCast(point1, point2, collectRadius, mainCamera.transform.forward, 
                    out RaycastHit hit, collectRange)) {
                BaseInteractableItem item = hit.collider.GetComponentInParent<BaseInteractableItem>();
                if (!item) return;
                
                if (item.IsCollectible && item != _currentItem)
                {
                    if (pickupText)
                    {
                        pickupText.text = $"Pick up '{item.name}' (E)";
                        pickupText.gameObject.SetActive(true);
                    }
                    HandleCollectInput(item);
                }
                else if (!item.IsCollectible)
                {
                    if (pickupText)
                    {
                        pickupText.text = $"Use '{item.name}' (E)";
                        pickupText.gameObject.SetActive(true);
                    }
                    
                    if (inputHandler.CollectInput)
                    {
                        inputHandler.ResetCollectInput();
                        item.Switch();
                    }
                }
            }
        }

        // Handle the item pickup process
        private void PickUpItem(BaseInteractableItem item) {
            
            if (!item || !item.IsCollectible) return;
            
            CollectItem(item);
            EquipItem();
            Destroy(item.gameObject);                                        // Remove item on the ground
            if (pickupText) pickupText.gameObject.SetActive(false);
        }

        // Toggle equipped / unequipped state
        private void HandleEquipInput() {
            if (inputHandler.EquipItemInput) {
                inputHandler.ResetEquipItemInput();                          // Avoid double activation

                _isEquipped = !_isEquipped;
                if (_isEquipped) EquipItem();
                else UnequipItem();
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        // Activate the current equipped item
        private void HandleActivateInput() {
            if (_isEquipped && inputHandler.FireInput) {
                inputHandler.ResetFireInput();
                _currentItem.ActivateComponent();
            }
        }

        // Drop the item when pressing the drop input
        private void HandleDropInput() {
            if (inputHandler.DropItemInput) {
                inputHandler.ResetDropItemInput();
                DropItem();
            }
        }

        // Handle pickup input when near an item
        private void HandleCollectInput(BaseInteractableItem item) {
            if (inputHandler.CollectInput) {
                inputHandler.ResetCollectInput();
                PickUpItem(item);
            }
        }
    }
}
