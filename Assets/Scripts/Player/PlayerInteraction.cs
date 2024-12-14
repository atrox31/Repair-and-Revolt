using Assets.Scripts.Extensions;
using Assets.Scripts.Systems.EventSystem;
using Assets.Scripts.Systems.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private readonly float _interactRange = 5f;

        private PlayerInput _playerInput;
        private Transform _source;
        private InputAction _interactInputAction;
        private InputAction _useInputAction;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _interactInputAction = _playerInput.actions["Interact"];
            _useInputAction = _playerInput.actions["Use"];
            _source = GetComponentInChildren<Camera>().transform;
        }

        private void OnEnable()
        {
            _interactInputAction.performed += HandleOnInteract;
            _useInputAction.performed += HandleOnUse;
            EventManager.Player.OnItemUsed += HandleOnItemUsed;
        }

        private void OnDisable()
        {
            _interactInputAction.performed -= HandleOnInteract;
            _useInputAction.performed -= HandleOnUse;
            EventManager.Player.OnItemUsed -= HandleOnItemUsed;
        }

        private void HandleOnInteract(InputAction.CallbackContext obj)
        {
            var interactable = _source.GetTargetAsInteractable(_interactRange);
            interactable?.Interact(_source);
        }

        private void HandleOnUse(InputAction.CallbackContext obj)
        {
            EventManager.Player.OnItemUse.Invoke();
        }

        private void HandleOnItemUsed(Item item)
        {
            Debug.Log($"Used item {item.Name}");
        }
    }
}
