using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaRegenRate = 10f; // 초당 회복량
    public float staminaRegenDelay = 3f; // 회복 시작까지 대기 시간
    public float sprintStaminaCost = 20f; // 초당 스프린트 스태미나 소모량
    public float jumpStaminaCost = 30f; // 점프 시 스태미나 소모량
    public float slideStaminaCost = 25f; // 슬라이드 시 스태미나 소모량

    [Header("Health Settings")]
    public float maxHealth = 100f;
}
