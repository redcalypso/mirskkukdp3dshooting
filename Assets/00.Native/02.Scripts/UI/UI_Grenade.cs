using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Grenade : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI grenadeText;
    [SerializeField] private Image chargeRing;
    [SerializeField] private Color fillColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0f);

    private float _chargeTimer;
    private float _maxChargeTime = 3f;
    private bool _isCharging;

    private void Awake()
    {
        if (grenadeText == null) grenadeText = GetComponent<TextMeshProUGUI>();
        if (chargeRing == null) chargeRing = GetComponentInChildren<Image>();
        
        if (chargeRing != null)
        {
            chargeRing.type = Image.Type.Filled;
            chargeRing.fillMethod = Image.FillMethod.Radial360;
            chargeRing.fillOrigin = (int)Image.Origin360.Top;
            chargeRing.fillClockwise = true;
            chargeRing.fillAmount = 0f;
            chargeRing.color = emptyColor;
            chargeRing.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (_isCharging)
        {
            _chargeTimer += Time.deltaTime;
            float fillAmount = Mathf.Clamp01(_chargeTimer / _maxChargeTime);
            chargeRing.fillAmount = fillAmount;
            chargeRing.color = Color.Lerp(emptyColor, fillColor, fillAmount);

            if (_chargeTimer >= _maxChargeTime)
            {
                StopChargeUI();
            }
        }
    }

    public void UpdateGrenadeText(int currentGrenades, int maxGrenades)
    {
        if (grenadeText != null) grenadeText.text = $"{currentGrenades}/{maxGrenades}";
    }

    public void StartChargeUI()
    {
        if (chargeRing == null) return;

        _isCharging = true;
        _chargeTimer = 0f;
        chargeRing.fillAmount = 0f;
        chargeRing.color = emptyColor;
        chargeRing.gameObject.SetActive(true);
    }

    public void StopChargeUI()
    {
        if (chargeRing == null) return;

        _isCharging = false;
        chargeRing.fillAmount = 0f;
        chargeRing.color = emptyColor;
        chargeRing.gameObject.SetActive(false);
    }

    public float GetChargeRatio()
    {
        return Mathf.Clamp01(_chargeTimer / _maxChargeTime);
    }
} 