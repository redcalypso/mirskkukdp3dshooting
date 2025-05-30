using UnityEngine;

public class PlayerMovingFunction : PlayerComponent
{
    //requirements
    private CharacterController _characterController;
    private Camera_CameraController _cameraController; 
    private PlayerStaminaFunction _staminaManager;
    private Animator _animator;

    [Header("Player Stats")]
    [SerializeField] private PlayerStats _playerStats;

    [Header("Movement Settings")]
    [SerializeField] private float _playerMoveSpeed = 5f;
    [SerializeField] private float _playerSprintSpeed = 8f;
    [SerializeField] private float _playerJumpForce = 2f;
    [SerializeField] private int _playerMaxJumpCount = 1;

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

    // Animation Parameters
    private const string BLEND_X = "BlendX";
    private const string BLEND_Y = "BlendY";
    private const string ROLL_TRIGGER = "Roll";
    private const string JUMP_TRIGGER = "Jump";
    private const string SPEED = "Speed";
    private const string IS_FALLING = "IsFalling";

    // moving variables
    private Vector3 _moveDirection;
    private float _currentSpeed;
    private int _currentJumpCount;
    [SerializeField] private Vector3 _velocity;
    private const float GRAVITY = -9.81f;
    private bool _isFalling;
    private const float TERMINAL_VELOCITY = -20f; // 최대 낙하 속도

    protected override void Awake()
    {
        base.Awake();
        if (_player == null) return;

        _characterController = GetComponent<CharacterController>();
        _cameraController = Camera.main.GetComponent<Camera_CameraController>();
        _staminaManager = GetComponent<PlayerStaminaFunction>();
        _animator = GetComponentInChildren<Animator>();

        if (_characterController == null || _cameraController == null || _staminaManager == null || _animator == null)
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
        HandleJump();

        // CheckWallCollision();
        // HandleWallClimbing();
        // HandleSlide();

        if (!_isWallClimbing)
        {
            _velocity.y += GRAVITY * Time.deltaTime;
            if (_velocity.y < TERMINAL_VELOCITY) _velocity.y = TERMINAL_VELOCITY;
        }

        Vector3 horizontalMove = _moveDirection * _currentSpeed;
        Vector3 totalMove = new Vector3(horizontalMove.x, _velocity.y, horizontalMove.z);

        _characterController.Move(totalMove * Time.deltaTime);

        if (!_characterController.isGrounded && _velocity.y < -0.1f)
        {
            if(!_isFalling)
            {
                _isFalling = true;
                _animator.SetBool(IS_FALLING, true);
                _velocity.y = -0.1f;
            }
        }
        else if(_characterController.isGrounded)
        {
            if(_isFalling)
            {
                _isFalling = false;
                _animator.SetBool(IS_FALLING, false);
            }
        }

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
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 moveInput = new Vector2(horizontal, vertical);

        if (_animator != null)
        {
            _animator.SetFloat(BLEND_X, horizontal);
            _animator.SetFloat(BLEND_Y, vertical);
            
            // Calculate and set speed parameter (0 to 1)
            float speedValue = moveInput.magnitude;
            if (Input.GetKey(KeyCode.LeftShift) && _staminaManager.CanUseStamina(_player.PlayerStats.SprintStaminaCost * Time.deltaTime))
            {
                speedValue = 1f; // Sprint is always at max speed
            }
            _animator.SetFloat(SPEED, speedValue);
        }

        if (moveInput.magnitude > 0.1f)
        {
            Vector3 dir = new Vector3(moveInput.x, 0, moveInput.y);
            if (_cameraController.CurrentMode != CameraMode.TDV)
                _moveDirection = Camera.main.transform.TransformDirection(dir).normalized;
            else
                _moveDirection = dir.normalized;

            // sprint
            if (Input.GetKey(KeyCode.LeftShift) && _staminaManager.CanUseStamina(_player.PlayerStats.SprintStaminaCost * Time.deltaTime))
            {
                _currentSpeed = _playerSprintSpeed;
                _staminaManager.UseStamina(_player.PlayerStats.SprintStaminaCost * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.LeftShift) && _staminaManager.CanUseStamina(_player.PlayerStats.SprintStaminaCost)) UI_Stamina.Instance.OnStaminaUseFailed();
            else _currentSpeed = _playerMoveSpeed;
        }
        else 
        {
            _moveDirection = Vector3.zero;

            // Reset blend parameter
            if (_animator != null)
            {
                _animator.SetFloat(BLEND_X, 0);
                _animator.SetFloat(BLEND_Y, 0);
                _animator.SetFloat(SPEED, 0);
            }
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_currentJumpCount < _playerMaxJumpCount && _staminaManager.CanUseStamina(_player.PlayerStats.JumpStaminaCost))
            {
                _velocity.y = Mathf.Sqrt(_playerJumpForce * -2.5f * GRAVITY);
                _currentJumpCount++;
                _staminaManager.UseStamina(_player.PlayerStats.JumpStaminaCost);

                if (_animator != null) _animator.SetTrigger(JUMP_TRIGGER);
            }
            else if (!_staminaManager.CanUseStamina(_player.PlayerStats.JumpStaminaCost)) UI_Stamina.Instance.OnStaminaUseFailed();
        }

        if (_characterController.collisionFlags == CollisionFlags.Below) _currentJumpCount = 0;
    }

    private void HandleWallClimbing()
    {
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
        if (_slideCooldownTimer > 0) _slideCooldownTimer -= Time.deltaTime;

        if (_isSliding)
        {
            _slideTimer -= Time.deltaTime;
            if (_slideTimer <= 0)
            {
                _isSliding = false;
                // Reset blend parameter
                if (_animator != null)
                {
                    _animator.SetFloat(BLEND_X, 0);
                    _animator.SetFloat(BLEND_Y, 0);
                    _animator.SetFloat(SPEED, 0);
                }
            }
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

                // Trigger roll animation
                if (_animator != null)
                {
                    _animator.SetTrigger(ROLL_TRIGGER);
                }
            }
        }
    }
} 