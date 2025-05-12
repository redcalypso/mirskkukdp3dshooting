using UnityEngine;
using UnityEngine.UI;

public class UI_Stamina : MonoBehaviour
{
    public static UI_Stamina Instance;

    [SerializeField] private PlayerStaminaFunction _staminaManager;
    [SerializeField] private Image _staminaRing;
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color lowColor = Color.red;

    private float _lastFill = 1f;
    private float _fadeAlpha = 1f;
    private float _fadeDuration = 1f;
    private float _fadeTimer = 0f;
    private bool _isFadingOut = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (_staminaManager == null || _staminaRing == null) return;

        float fill = _staminaManager.MaxStamina > 0 ? _staminaManager.CurrentStamina / _staminaManager.MaxStamina : 0f;
        _staminaRing.fillAmount = fill;

        Color targetColor = fill <= 0.5f ? Color.Lerp(normalColor, lowColor, 1 - fill * 2) : normalColor;
        _staminaRing.color = new Color(targetColor.r, targetColor.g, targetColor.b, _staminaRing.color.a);

        if (fill >= 0.95f)
        {
            if (!_isFadingOut)
            {
                _fadeTimer = 0f;
                _isFadingOut = true;
            }
            if (_fadeAlpha > 0f)
            {
                _fadeTimer += Time.deltaTime;
                _fadeAlpha = Mathf.Clamp01(1f - (_fadeTimer / _fadeDuration));
            }
        }
        else
        {
            _isFadingOut = false;
            _fadeAlpha = 1f;
        }

        // 알파 적용
        Color c = _staminaRing.color;
        c.a = _fadeAlpha;
        _staminaRing.color = c;

        _lastFill = fill;
    }

    public void OnStaminaUseFailed()
    {
        // (스프링 이펙트는 주석 처리 상태)
    }
}
