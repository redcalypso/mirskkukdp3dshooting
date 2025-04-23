using UnityEngine;

public class PlayerStaminaManager : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    
    private float currentStamina;
    private float lastStaminaUseTime;
    private bool isInitialized;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => playerStats.maxStamina;

    private void Start()
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not assigned!");
            return;
        }
        Initialize();
    }

    private void Initialize()
    {
        currentStamina = playerStats.maxStamina;
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
        if (Time.time - lastStaminaUseTime >= playerStats.staminaRegenDelay)
        {
            currentStamina = Mathf.Min(playerStats.maxStamina, 
                currentStamina + playerStats.staminaRegenRate * Time.deltaTime);
        }
    }
} 