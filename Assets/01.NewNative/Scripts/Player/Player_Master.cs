using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Master : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    [Header("References")]
    [SerializeField] private Transform _cameraHandler;
    [SerializeField] private CharacterController _characterController;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 8f;
    [SerializeField] private float _rotationSpeed = 10f;

    // Input System
    private InputSystem_Actions _inputActions;
    private Vector2 _moveInput;
    private bool _isSprinting;
    private Vector3 _moveDirection;
    private float _currentSpeed;

    public Transform CameraHandler => _cameraHandler;
    public CharacterController CharacterController => _characterController;
    public float CurrentSpeed => _currentSpeed;
    public Vector3 MoveDirection => _moveDirection;

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _inputActions.Player.SetCallbacks(this);
        _currentSpeed = _moveSpeed;
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        _isSprinting = context.performed;
        _currentSpeed = _isSprinting ? _sprintSpeed : _moveSpeed;
    }

    public void SetMoveDirection(Vector3 direction)
    {
        _moveDirection = direction;
    }

    public void Move()
    {
        if (_characterController != null && _moveDirection.magnitude > 0.1f)
        {
            _characterController.Move(_moveDirection * _currentSpeed * Time.deltaTime);
        }
    }

    public void RotateTowards(Vector3 direction)
    {
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    // Required interface methods that we don't use
    public void OnLook(InputAction.CallbackContext context) { }
    public void OnAttack(InputAction.CallbackContext context) { }
    public void OnInteract(InputAction.CallbackContext context) { }
    public void OnCrouch(InputAction.CallbackContext context) { }
    public void OnJump(InputAction.CallbackContext context) { }
    public void OnPrevious(InputAction.CallbackContext context) { }
    public void OnNext(InputAction.CallbackContext context) { }
}
