using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Ammo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Image reloadRing;
    [SerializeField] private Color fillColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0f);

    private float _reloadTimer;
    private float _maxReloadTime;
    private bool _isReloading;

    private void Awake()
    {
        if (ammoText == null) ammoText = GetComponent<TextMeshProUGUI>();
        if (reloadRing == null) reloadRing = GetComponentInChildren<Image>();
        
        if (reloadRing != null)
        {
            reloadRing.type = Image.Type.Filled;
            reloadRing.fillMethod = Image.FillMethod.Radial360;
            reloadRing.fillOrigin = (int)Image.Origin360.Top;
            reloadRing.fillClockwise = true;
            reloadRing.fillAmount = 0f;
            reloadRing.color = emptyColor;
            reloadRing.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (_isReloading)
        {
            _reloadTimer -= Time.deltaTime;
            float fillAmount = 1f - (_reloadTimer / _maxReloadTime);
            reloadRing.fillAmount = fillAmount;
            reloadRing.color = Color.Lerp(emptyColor, fillColor, fillAmount);

            if (_reloadTimer <= 0)
            {
                StopReloadUI();
            }
        }
    }

    public void UpdateAmmoText(int currentAmmo, int maxAmmo)
    {
        if (ammoText != null) ammoText.text = $"{currentAmmo:D2}/{maxAmmo:D2}";
    }

    public void StartReloadUI(float reloadTime)
    {
        if (reloadRing == null) return;

        _isReloading = true;
        _reloadTimer = reloadTime;
        _maxReloadTime = reloadTime;
        reloadRing.fillAmount = 0f;
        reloadRing.color = emptyColor;
        reloadRing.gameObject.SetActive(true);
    }

    public void StopReloadUI()
    {
        if (reloadRing == null) return;

        _isReloading = false;
        reloadRing.fillAmount = 0f;
        reloadRing.color = emptyColor;
        reloadRing.gameObject.SetActive(false);
    }
} 