using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPreset", menuName = "Enemy/EnemyPreset")]
public class EnemyPreset : ScriptableObject
{
    [Header("Stats")]
    public float FindDistance = 10f;
    public float AttackDistance = 2f;
    public int AttackDamage = 10;
    public float AttackCoolTime = 1f;
    public float MaxHealth = 100f;
    
    [Header("Patrol Settings")]
    public float PatrolRadius = 10f;
    public float MinPatrolWaitTime = 2f;
    public float MaxPatrolWaitTime = 5f;
    public float IdleToPatrolTime = 3f;

    [Header("Knock Back Settings")]
    public float KnockbackForce = 10f;
    public float KnockbackDuration = 0.3f;
}
