using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum EnemyState
{
    Idle,
    Track,
    Patrol,
    Return,
    Attack,
    Die
}

public class Enemy : MonoBehaviour, IDamageable
{
    // requirements
    [SerializeField] private EnemyPreset _enemyPreset;
    [SerializeField] private EnemyState _enemyState;
    [SerializeField] private Transform _player;
    [SerializeField] private Animator _animator;

    [Header("Health Bar UI")]
    [SerializeField] private Canvas _healthBarCanvas;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private float _healthBarHeight = 2f;

    private NavMeshAgent _agent;
    private Vector3 _startPosition;
    private float _currentHealth;
    private bool _isKnockback;
    private float _knockbackEndTime;
    private float _lastAttackTime; // 마지막 공격 시간

    public bool IsDead => _currentHealth <= 0;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _startPosition = transform.position;
        _currentHealth = _enemyPreset.MaxHealth;
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (_player == null)
        {
            Debug.LogWarning("Player Tag Missing");
        }

        _enemyState = EnemyState.Idle;
        InitializeHealthBar();
    }

    private void InitializeHealthBar()
    {
        if (_healthSlider != null)
        {
            _healthSlider.maxValue = _enemyPreset.MaxHealth;
            _healthSlider.value = _currentHealth;
            _healthBarCanvas.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (_isKnockback)
        {
            if (Time.time >= _knockbackEndTime)
            {
                _isKnockback = false;
                _agent.enabled = true;
            }
            return;
        }

        switch (_enemyState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Track:
                Track();
                break;
            case EnemyState.Return:
                Return();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Die:
                Die();
                break;
        }
    }

    private void Idle()
    {
        if (_player != null && Vector3.Distance(transform.position, _player.position) < _enemyPreset.FindDistance) _enemyState = EnemyState.Track;
    }

    private void Track()
    {
        if (_player == null) return;

        _agent.SetDestination(_player.position);

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        if (distanceToPlayer > _enemyPreset.FindDistance)  _enemyState = EnemyState.Return;
        else if (distanceToPlayer <= _enemyPreset.AttackDistance) _enemyState = EnemyState.Attack;
    }

    private void Return()
    {
        _agent.SetDestination(_startPosition);

        if (Vector3.Distance(transform.position, _startPosition) <= 0.1f) _enemyState = EnemyState.Idle;
    }

    private void Attack()
    {
        if (_player == null) return;

        if (Vector3.Distance(transform.position, _player.position) > _enemyPreset.AttackDistance) _enemyState = EnemyState.Track;
        else
        {
            if (Time.time >= _lastAttackTime + _enemyPreset.AttackCoolTime)
            {
                IDamageable playerDamageable = _player.GetComponent<IDamageable>();
                if (playerDamageable != null)
                {
                    playerDamageable.TakeDamage(_enemyPreset.AttackDamage);
                    _lastAttackTime = Time.time;
                }
            }
        }
    }

    private void Die()
    {
        // TODO: 사망 효과 추가
        gameObject.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        _currentHealth -= damage;
        UpdateHealthBar();
        
        if (IsDead) _enemyState = EnemyState.Die;
        else
        {
            ApplyKnockback();
            _enemyState = EnemyState.Track;
        }
    }

    private void UpdateHealthBar()
    {
        if (_healthSlider == null) return;

        _healthSlider.value = _currentHealth;
        _healthBarCanvas.gameObject.SetActive(_currentHealth < _enemyPreset.MaxHealth);
    }

    private void ApplyKnockback()
    {
        if (_player == null) return;

        _isKnockback = true;
        _agent.enabled = false;
        _knockbackEndTime = Time.time + _enemyPreset.KnockbackDuration;

        Vector3 knockbackDirection = (transform.position - _player.position).normalized;
        knockbackDirection.y = 0;
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.AddForce(knockbackDirection * _enemyPreset.KnockbackForce, ForceMode.Impulse);
    }

    private void OnValidate()
    {
        if (_healthBarCanvas != null)
        {
            Vector3 position = transform.position;
            position.y += _healthBarHeight;
            _healthBarCanvas.transform.position = position;
        }
    }
} 