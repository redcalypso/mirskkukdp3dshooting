using UnityEngine;

[CreateAssetMenu(fileName = "GunPreset", menuName = "Player/Gun Preset")]
public class GunPreset : ScriptableObject
{
    [Header("Gun Settings")]
    public float fireRate = 0.1f;
    public int maxAmmo = 30;
    public int currentAmmo = 30;
    public float reloadTime = 2f;
    public float damage = 10f;
    public float range = 100f;
    public LayerMask shootableLayers;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem hitFlash;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;
} 