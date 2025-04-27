using UnityEngine;
public class PlayerMovingFunction : PlayerComponent
{
    //requirements
    private CharacterController _characterController;
    private Camera_CameraController _cameraController; 
    private PlayerStaminaFunction _staminaManager;

    [Header("Player Stats")]
    [SerializeField] private PlayerStats _playerStats;

    [Header("Movement Settings")]
    [SerializeField] private float _playerMoveSpeed = 5f;
    [SerializeField] private float _playerSprintSpeed = 8f;
    [SerializeField] private float _playerJumpForce = 2f;
    [SerializeField] private int _playerMaxJumpCount = 2;

    [Header("Slide Settings")]
    [SerializeField] private float _slideSpeed = 12f;
    [SerializeField] private float _slideDuration = 0.5f;
    [SerializeField] private float _slideCooldown = 1f;
    private bool _isSliding;
    private float _slideTimer;
    private float _slideCooldownTimer;

    [Header("Wall Climbing Settings")]
    [SerializeField] private float _wallClimbSpeed = 2f;
    [SerializeField] private float _wallCheckDistance = 0.6f;
    [SerializeField] private LayerMask _wallLayer;
    private bool _isWallClimbing;
    private bool _isTouchingWall;

    // moving variables
    private Vector3 _moveDirection;
    private float _currentSpeed;
    private int _currentJumpCount;
    private Vector3 _velocity;
    private const float GRAVITY = -9.81f;

    protected override void Awake()
    {
        base.Awake();
        if (_player == null) return;

        _characterController = GetComponent<CharacterController>();
        _cameraController = Camera.main.GetComponent<Camera_CameraController>();
        _staminaManager = GetComponent<PlayerStaminaFunction>();

        if (_characterController == null || _cameraController == null || _staminaManager == null)
        {
            Debug.LogError($"[{GetType().Name}] Required components not found!");
            return;
        }

        _currentSpeed = _playerMoveSpeed;
        _currentJumpCount = 0;
    }

    private void Update()
    {
        if (_player == null || _staminaManager == null) return;

        HandleMovement();
        CheckWallCollision();
        HandleWallClimbing();
        HandleJump();
        HandleSlide();

        if (_moveDirection.magnitude > 0.1f && !_isSliding) _characterController.Move(_moveDirection * _currentSpeed * Time.deltaTime);

        if (!_isWallClimbing) _velocity.y += GRAVITY * Time.deltaTime;

        _characterController.Move(_velocity * Time.deltaTime);

        if (_characterController.collisionFlags == CollisionFlags.Below) _currentJumpCount = 0;
    }

    private void CheckWallCollision()
    {
        if (_player == null) return;

        RaycastHit hit;
        Vector3 forward = transform.forward;
        
        _isTouchingWall = Physics.Raycast(transform.position, forward, out hit, _wallCheckDistance, _wallLayer) || Physics.Raycast(transform.position + Vector3.up * 0.5f, forward, out hit, _wallCheckDistance, _wallLayer);
    }

    private void HandleMovement()
    {
        if (_player == null || _staminaManager == null) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 moveInput = new Vector2(horizontal, vertical);

        if (moveInput.magnitude > 0.1f)
        {
            Vector3 dir = new Vector3(moveInput.x, 0, moveInput.y);
            _moveDirection = Camera.main.transform.TransformDirection(dir).normalized;
            _moveDirection.y = 0;

            // sprint
            if (Input.GetKey(KeyCode.LeftShift) && _staminaManager.CanUseStamina(_player.PlayerStats.SprintStaminaCost * Time.deltaTime))
            {
                _currentSpeed = _playerSprintSpeed;
                _staminaManager.UseStamina(_player.PlayerStats.SprintStaminaCost * Time.deltaTime);
            }
            else _currentSpeed = _playerMoveSpeed;
        }
        else _moveDirection = Vector3.zero;
    }

    private void HandleJump()
    {
        if (_player == null || _staminaManager == null) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_currentJumpCount < _playerMaxJumpCount && _staminaManager.CanUseStamina(_player.PlayerStats.JumpStaminaCost))
            {
                _velocity.y = Mathf.Sqrt(_playerJumpForce * -1f * GRAVITY);
                _currentJumpCount++;
                _staminaManager.UseStamina(_player.PlayerStats.JumpStaminaCost);
            }
        }
    }

    private void HandleWallClimbing()
    {
        if (_player == null || _staminaManager == null) return;

        if (Input.GetKey(KeyCode.Space))
        {
            if (_staminaManager.CanUseStamina(_wallClimbSpeed * Time.deltaTime) && _isTouchingWall)
            {
                _velocity.y = _wallClimbSpeed;
                _staminaManager.UseStamina(_wallClimbSpeed * Time.deltaTime);
                _isWallClimbing = true;
            }
            else _isWallClimbing = false;
        }
        else _isWallClimbing = false;
    }

    private void HandleSlide()
    {
        if (_player == null || _staminaManager == null) return;

        if (_slideCooldownTimer > 0) _slideCooldownTimer -= Time.deltaTime;

        if (_isSliding)
        {
            _slideTimer -= Time.deltaTime;
            if (_slideTimer <= 0)
                _isSliding = false;
            else _characterController.Move(_moveDirection * _slideSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.C) && !_isSliding && _slideCooldownTimer <= 0 && _moveDirection.magnitude > 0.1f)
        {
            if (_staminaManager.CanUseStamina(_player.PlayerStats.SlideStaminaCost))
            {
                _isSliding = true;
                _slideTimer = _slideDuration;
                _slideCooldownTimer = _slideCooldown;
                _staminaManager.UseStamina(_player.PlayerStats.SlideStaminaCost);
            }
        }
    }
} 