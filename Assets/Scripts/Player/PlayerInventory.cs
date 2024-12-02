using Assets.Scripts.Systems.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInventory : MonoBehaviour
    {
        private const string ActionName = "InventorySlotChange";

        [SerializeField] private InventoryView _view;
        [SerializeField] private int _capacity = 6;

        private PlayerInput _playerInput;
        private InventoryController _inventoryController;
        private int _activeInventorySlot = 0;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _inventoryController = new InventoryController(_view, _capacity);
        }

        private void OnEnable()
        {
            _playerInput.actions[ActionName].performed += HandleOnInventorySlotChange;
        }

        private void OnDisable()
        {
            _playerInput.actions[ActionName].performed -= HandleOnInventorySlotChange;
        }

        private void HandleOnInventorySlotChange(InputAction.CallbackContext obj)
        {
            var name = obj.control.displayName;

            if (name == "Scroll Down")
            {
                var value = (int)obj.ReadValue<float>();
                var result = _activeInventorySlot + value;

                if (result < 0) _activeInventorySlot = _capacity - 1;
                else if (result >= _capacity) _activeInventorySlot = 0;
                else _activeInventorySlot = result;
            }
            else
            {
                _activeInventorySlot = int.Parse(name) - 1;
            }

            _inventoryController.SwitchInventorySlot(_activeInventorySlot);
        }
    }
}
