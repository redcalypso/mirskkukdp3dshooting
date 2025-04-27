using UnityEngine;

[CreateAssetMenu(fileName = "GunPreset", menuName = "Player/Gun Preset")]
public class Weapon_GunPreset : ScriptableObject
{
    [Header("Gun Settings")]
    public float fireRate = 0.1f;
    public int maxAmmo = 30;
    public float reloadTime = 2f;
    public int damage = 30;
    public float range = 100f;

    [Header("Recoil Settings")]
    public float recoilAmount = 1f;
    public float recoilDuration = 0.1f;
    public Vector2[] recoilPattern = new Vector2[]
    {
        new Vector2(0, 2),
        new Vector2(2, 2),
        new Vector2(-2, 2),
        new Vector2(0, 2),
        new Vector2(2, 2),
        new Vector2(-2, 2),
        new Vector2(0, 2),
        new Vector2(2, 2),
        new Vector2(-2, 2),
        new Vector2(0, 2),
    };

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem hitFlash;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;
} 