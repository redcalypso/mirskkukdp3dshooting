using UnityEngine;

[CreateAssetMenu(fileName = "GunPreset", menuName = "Player/Gun Preset")]
public class GunPreset : ScriptableObject
{
    [Header("Gun Settings")]
    public float fireRate = 0.1f;
    public int maxAmmo = 30;
    public float reloadTime = 2f;
    public float damage = 10f;
    public float range = 100f;

    [Header("Recoil Settings")]
    public float recoilAmount = 0.1f;
    public float recoilDuration = 0.1f;
    public Vector2[] recoilPattern = new Vector2[]
    {
        new Vector2(0, 2),    // 첫 발: 위로
        new Vector2(2, 2),    // 두 번째: 오른쪽 위
        new Vector2(-2, 2),   // 세 번째: 왼쪽 위
        new Vector2(0, 2),    // 네 번째: 위로
        new Vector2(2, 2),    // 다섯 번째: 오른쪽 위
        new Vector2(-2, 2),   // 여섯 번째: 왼쪽 위
        new Vector2(0, 2),    // 일곱 번째: 위로
        new Vector2(2, 2),    // 여덟 번째: 오른쪽 위
        new Vector2(-2, 2),   // 아홉 번째: 왼쪽 위
        new Vector2(0, 2),    // 열 번째: 위로
    };

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem hitFlash;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;
} 