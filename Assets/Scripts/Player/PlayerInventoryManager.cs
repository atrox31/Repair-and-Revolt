using Assets.Scripts.Interfaces;
using Assets.Scripts.Systems.EventSystem;
using Assets.Scripts.Systems.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInventoryManager : MonoBehaviour
    {
        private const string ActionName = "InventorySlotChange";

        [SerializeField] private InventoryView _view;
        [SerializeField] private int _capacity = 6;

        private PlayerInput _playerInput;
        private InventoryController _inventoryController;
        private int _activeInventorySlot;
        private Item _activeItem => _view.Slots[_activeInventorySlot].Item;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _inventoryController = new InventoryController(_view, _capacity);
        }

        private void OnEnable()
        {
            _playerInput.actions[ActionName].performed += HandleOnInventorySlotChange;
            EventManager.Player.OnItemPicked += HandleOnItemPicked;
            EventManager.Player.OnItemUse += HandleOnItemUse;
        }

        private void OnDisable()
        {
            _playerInput.actions[ActionName].performed -= HandleOnInventorySlotChange;
            EventManager.Player.OnItemPicked -= HandleOnItemPicked;
            EventManager.Player.OnItemUse -= HandleOnItemUse;
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

        private void HandleOnItemPicked(GameObject player, IInventoryItem inventoryItem)
        {
            _view.Slots[_activeInventorySlot].Set(inventoryItem.Item);
        }

        private void HandleOnItemUse()
        {
            if (_activeItem is null)
            {
                Debug.Log("No item to use");
                return;
            }

            EventManager.Player.OnItemUsed.Invoke(_activeItem);
        }
    }
}
