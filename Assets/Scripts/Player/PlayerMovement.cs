using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerMovement : MonoBehaviour
    {
        public Camera Camera;
        public float WalkSpeed = 6f;
        public float RunSpeed = 12f;
        public float JumpPower = 7f;
        public float Gravity = 10f;
        public float LookSpeed = 2f;
        public float LookXLimit = 45f;
        public float DefaultHeight = 2f;
        public float CrouchHeight = 1f;
        public float CrouchSpeed = 3f;

        private PlayerInput _playerInput;
        private CharacterController _characterController;
        private Vector3 _moveDirection = Vector3.zero;
        private float _rotationX = 0;
        private bool _canMove = true;
        private bool _isCrouching = false;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnEnable()
        {
            _playerInput.actions["Crouch"].performed += Crouch;
        }

        private void OnDisable()
        {
            _playerInput.actions["Crouch"].performed -= Crouch;
        }

        private void Update()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = _canMove ? (isRunning ? RunSpeed : WalkSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = _canMove ? (isRunning ? RunSpeed : WalkSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = _moveDirection.y;
            _moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump") && _canMove && _characterController.isGrounded)
            {
                _moveDirection.y = JumpPower;
            }
            else
            {
                _moveDirection.y = movementDirectionY;
            }

            if (!_characterController.isGrounded)
            {
                _moveDirection.y -= Gravity * Time.deltaTime;
            }

            _characterController.Move(_moveDirection * Time.deltaTime);

            if (_canMove)
            {
                _rotationX += -Input.GetAxis("Mouse Y") * LookSpeed;
                _rotationX = Mathf.Clamp(_rotationX, -LookXLimit, LookXLimit);
                Camera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * LookSpeed, 0);
            }
        }

        private void Crouch(InputAction.CallbackContext obj)
        {
            _isCrouching = !_isCrouching;
            _characterController.height = _isCrouching ? CrouchHeight : DefaultHeight;
            WalkSpeed = _isCrouching ? CrouchSpeed : WalkSpeed;
            RunSpeed = _isCrouching ? CrouchSpeed : RunSpeed;
        }
    }
}
