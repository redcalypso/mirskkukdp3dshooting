using UnityEngine;

public class PlayerStaminaFunction : PlayerComponent
{
    // For Referenceing
    public float MaxStamina => _player.PlayerStats.MaxStamina;
    public float CurrentStamina => _currentStamina;

    // Variables
    private float _currentStamina;
    private float _lastStaminaUse;
    private bool _isInitialized;
    

    protected override void Awake()
    {
        base.Awake();

        if (_player == null || _player.PlayerStats == null)
        {
            Debug.LogError($"[{GetType().Name}] Player or PlayerStats not found!");
            return;
        }

        Initialize();
    }

    private void Initialize()
    {
        _currentStamina = _player.PlayerStats.MaxStamina;
        _lastStaminaUse = 0f;
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized) return;
        UpdateStamina();
    }

    public bool CanUseStamina(float cost)
    {
        if (!_isInitialized) return false;
        return _currentStamina >= cost;
    }

    public void UseStamina(float amount)
    {
        if (!_isInitialized) return;
        _currentStamina = Mathf.Max(0f, _currentStamina - amount);
        _lastStaminaUse = Time.time;
    }

    private void UpdateStamina()
    {
        if (!_isInitialized || _player == null || _player.PlayerStats == null) return;

        if (Time.time - _lastStaminaUse >= _player.PlayerStats.StaminaRegenDelay)
        {
            _currentStamina = Mathf.Min(_player.PlayerStats.MaxStamina, _currentStamina + _player.PlayerStats.StaminaRegenRate * Time.deltaTime);
        }   
    }
} 