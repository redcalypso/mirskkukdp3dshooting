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

    private int _lastHealth;
    private Coroutine _bleedEffectCoroutine;

    private void Start()
    {
        if(_player == null)
        {
            Debug.LogWarning("Player component not found");
            return;
        }
        _lastHealth = _player.CurrentHealth;
        UpdateHealthUI();
    }

    private void Update()
    {
        if (_player != null)
        {
            if (_lastHealth != _player.CurrentHealth)
            {
                _lastHealth = _player.CurrentHealth;
                UpdateHealthUI();
            }
        }
    }

    private void UpdateHealthUI()
    {
        int currentHealth = _player.CurrentHealth;

        _healthText.text = currentHealth.ToString();

        float healthPercentage = (float)currentHealth / _player.PlayerStats.MaxHealth;
        _healthText.color = Color.Lerp(_healthTextColorEnd, _healthTextColorStart, healthPercentage);

        if (_bleedEffectCoroutine != null) StopCoroutine(_bleedEffectCoroutine);
        _bleedEffectCoroutine = StartCoroutine(PlayBleedEffect());
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

        bleedColor.a = 0f;
        _bleedFX.color = bleedColor;
        _bleedEffectCoroutine = null;
    }
}
