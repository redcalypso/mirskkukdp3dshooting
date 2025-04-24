using UnityEngine;

public class PlayerStaminaManager : PlayerComponent
{
    private float currentStamina;
    private float lastStaminaUseTime;
    private bool isInitialized;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => _player.playerStats.maxStamina;

    protected override void Awake()
    {
        base.Awake();
        if (_player == null || _player.playerStats == null)
        {
            Debug.LogError($"[{GetType().Name}] Player or PlayerStats not found!");
            return;
        }
        Initialize();
    }

    private void Initialize()
    {
        currentStamina = _player.playerStats.maxStamina;
        lastStaminaUseTime = 0f;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;
        UpdateStamina();
    }

    public bool CanUseStamina(float cost)
    {
        if (!isInitialized) return false;
        return currentStamina >= cost;
    }

    public void UseStamina(float amount)
    {
        if (!isInitialized) return;
        currentStamina = Mathf.Max(0f, currentStamina - amount);
        lastStaminaUseTime = Time.time;
    }

    private void UpdateStamina()
    {
        if (!isInitialized || _player == null || _player.playerStats == null) return;

        if (Time.time - lastStaminaUseTime >= _player.playerStats.staminaRegenDelay)
        {
            currentStamina = Mathf.Min(_player.playerStats.maxStamina, 
                currentStamina + _player.playerStats.staminaRegenRate * Time.deltaTime);
        }
    }
} 