using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;

public class UI_Health : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private RawImage _bleedFX;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private Color _healthTextColorStart = Color.white;
    [SerializeField] private Color _healthTextColorEnd = Color.red;
    [SerializeField] private float _bleedEffectDuration = 1f;
    [SerializeField] private AnimationCurve _bleedEffectCurve;

    private void Start()
    {
        if(_player == null)
        {
            Debug.LogWarning("Player component not found");
            return;
        }
        UpdateHealthUI();
    }

    private void Update()
    {
        if (_player != null)
        {
            UpdateHealthUI();
        }
    }

    private void UpdateHealthUI()
    {
        int currentHealth = _player.CurrentHealth;
        
        // Update health text
        _healthText.text = currentHealth.ToString();
        
        // Update text color based on health percentage
        float healthPercentage = (float)currentHealth / _player.PlayerStats.MaxHealth;
        _healthText.color = Color.Lerp(_healthTextColorEnd, _healthTextColorStart, healthPercentage);

        // Trigger bleed effect
        StartCoroutine(PlayBleedEffect());
    }

    private IEnumerator PlayBleedEffect()
    {
        Color bleedColor = _bleedFX.color;
        float elapsedTime = 0f;

        while (elapsedTime < _bleedEffectDuration)
        {
            float alpha = _bleedEffectCurve.Evaluate(elapsedTime / _bleedEffectDuration);
            bleedColor.a = alpha;
            _bleedFX.color = bleedColor;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset alpha to 0
        bleedColor.a = 0f;
        _bleedFX.color = bleedColor;
    }
}
