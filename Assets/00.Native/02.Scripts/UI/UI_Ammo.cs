using UnityEngine;
using TMPro;

public class UI_Ammo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;

    private void Awake()
    {
        if (ammoText == null) ammoText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateAmmoText(int currentAmmo, int maxAmmo)
    {
        if (ammoText != null) ammoText.text = $"{currentAmmo:D2}/{maxAmmo:D2}";
    }
} 