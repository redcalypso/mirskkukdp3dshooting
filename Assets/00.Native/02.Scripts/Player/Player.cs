using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [Header("Player Stats")]
    [SerializeField] public PlayerStats PlayerStats;
    private int _currentHealth = 100;
    public bool IsDead => _currentHealth <= 0;
    public int CurrentHealth => _currentHealth;

    private void Start()
    {
        _currentHealth = PlayerStats.MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        _currentHealth -= damage;
        
        if (IsDead)
        {
            FlowManager.Instance?.GameOver();
        }
    }
}