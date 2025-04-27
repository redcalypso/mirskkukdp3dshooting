using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Stamina : MonoBehaviour
{
    // requirements
    [SerializeField] private PlayerStaminaFunction _staminaManager;

    [Header("UI References")]
    [SerializeField] private Slider _staminaSlider;
    
    private void Start()
    {
        if (_staminaSlider != null)
        {
            _staminaSlider.maxValue = _staminaManager.MaxStamina;
            _staminaSlider.value = _staminaManager.CurrentStamina;
        }
    }

    private void Update()
    {
        if (_staminaManager == null) return;
        if (_staminaSlider != null)  _staminaSlider.value = _staminaManager.CurrentStamina;
    }
} 