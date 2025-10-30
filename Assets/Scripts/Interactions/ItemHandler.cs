using UnityEngine;
using Player;

namespace Items
{
    public class ItemHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform leftArm;               // Where the item is held when equipped
        [SerializeField] private PlayerInputHandler inputHandler; // InputSystem Script
        [SerializeField] private BaseInteractableItem itemPrefab; // Reference to the item prefab

        [Header("Hand Movement")]
        [SerializeField] private Vector3 equippedArmRotation = new Vector3(-90f, 0f, 0f);
        [SerializeField] private float handLiftSpeed = 5f;
        [SerializeField] private float handSwayAmplitude = 0.5f;
        [SerializeField] private float handSwaySpeed = 5f;

        private Quaternion _initialHandRotation;
        private BaseInteractableItem equippedItem;
        private bool _isEquipped;

        private void Start()
        {
            SpawnItem();

            if (leftArm) _initialHandRotation = leftArm.localRotation;
        }

        private void Update()
        {
            HandleEquipInput();
            HandleArmMovement();
        }

        private void SpawnItem() {                                          
            if (!itemPrefab || !leftArm) return;

            GameObject instance = Instantiate(itemPrefab.gameObject, leftArm);
            instance.transform.position = leftArm.position + leftArm.forward * 0.1f + leftArm.up * -0.1f;
            instance.transform.rotation = leftArm.rotation * Quaternion.Euler(90f, 0f, 0f);
            instance.transform.localScale = itemPrefab.transform.localScale;
            instance.SetActive(_isEquipped);

            equippedItem = instance.GetComponent<BaseInteractableItem>();
        }

        private void HandleEquipInput() {
            if (inputHandler.EquipInput) {
                _isEquipped = !_isEquipped;
                inputHandler.ResetEquipInput();

                if (equippedItem) equippedItem.gameObject.SetActive(_isEquipped);

                if (_isEquipped) equippedItem?.Switch();
                else equippedItem?.Switch();
            }
        }

        private void HandleArmMovement() {
            if (!leftArm) return;

            if (_isEquipped) {
                Quaternion targetRotation = Quaternion.Euler(equippedArmRotation);
                leftArm.localRotation = Quaternion.Slerp(leftArm.localRotation, targetRotation, Time.deltaTime * handLiftSpeed);

                float sway = Mathf.Sin(Time.time * handSwaySpeed) * handSwayAmplitude;
                leftArm.localRotation *= Quaternion.Euler(sway, 0f, 0f);
            } else {
                leftArm.localRotation = Quaternion.Slerp(leftArm.localRotation, _initialHandRotation, Time.deltaTime * handLiftSpeed);
            }
        }
    }
}
