using Assets.Scripts.Extensions;
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
        private InputAction _interactInputAsset;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _interactInputAsset = _playerInput.actions["Interact"];
            _source = GetComponentInChildren<Camera>().transform;
        }

        private void OnEnable()
        {
            _interactInputAsset.performed += OnPlayerInteract;
        }

        private void OnDisable()
        {
            _interactInputAsset.performed -= OnPlayerInteract;
        }

        private void OnPlayerInteract(InputAction.CallbackContext obj)
        {
            var interactable = _source.GetTargetAsInteractable(_interactRange);
            interactable?.Interact(_source);
        }
    }
}
