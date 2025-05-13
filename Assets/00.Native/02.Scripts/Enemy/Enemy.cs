using System.Collections;
using Unity.VisualScripting;
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
    private float _lastAttackTime; 
    private Vector3[] _patrolPoints;
    private int _currentPatrolIndex;
    private float _idleTimer;
    private float _idleDuration = 5f; // 몇 초 후 순찰 시작
    private bool _isWaitingAtPatrolPoint;
    private float _waitEndTime;
    [SerializeField] private float _patrolWaitDuration = 2f; // 정지 시간

    [Header("Patrol Settings (Dynamic)")]   
    [SerializeField] private float _patrolRadius = 10f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _raycastHeightOffset = 10f;
    [SerializeField] private float _raycastMaxDistance = 20f;

    // Animator Variable
    private const string ISATTACKING = "Attack";
    private const string HITREACTION = "HitReaction";
    private const string MOVINGSPEED = "Speed";
    private const string ENEMYDEATH = "Death";

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
        
        if (_player == null) Debug.LogWarning("Player Tag Missing");

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
            case EnemyState.Patrol:
                Patrol();
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
        
        _animator.SetFloat(MOVINGSPEED, 0);

        if (_player != null && Vector3.Distance(transform.position, _player.position) < _enemyPreset.FindDistance)
        {
            _enemyState = EnemyState.Track;
            _idleTimer = 0f;
            return;
        }

        _idleTimer += Time.deltaTime;
        if (_idleTimer >= _idleDuration)
        {
            _idleTimer = 0f;
            _enemyState = EnemyState.Patrol;
        }
    }
    
    private void Patrol()
    {
        if (_isWaitingAtPatrolPoint)
        {
            _animator.SetFloat(MOVINGSPEED, 0);

            if (Time.time >= _waitEndTime)
            {
                _isWaitingAtPatrolPoint = false;
                GenerateNextPatrolPoint();
            }
            return;
        }

        if (_patrolPoints == null || _patrolPoints.Length == 0)
        {
            GenerateNextPatrolPoint();
            return;
        }

        Vector3 targetPoint = _patrolPoints[_currentPatrolIndex];
        _agent.SetDestination(targetPoint);

        float speedValue = _agent.velocity.magnitude / _agent.speed;
        _animator.SetFloat(MOVINGSPEED, speedValue);

        if (Vector3.Distance(transform.position, targetPoint) < 0.5f)
        {
            _isWaitingAtPatrolPoint = true;
            _waitEndTime = Time.time + _patrolWaitDuration;
        }

        if (_player != null && Vector3.Distance(transform.position, _player.position) < _enemyPreset.FindDistance) _enemyState = EnemyState.Track;
    }

    private void Track()
    {
        if (_player == null) return;

        _agent.SetDestination(_player.position);

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        if (distanceToPlayer > _enemyPreset.FindDistance)  _enemyState = EnemyState.Return;
        else if (distanceToPlayer <= _enemyPreset.AttackDistance) _enemyState = EnemyState.Attack;

        float speedValue = _agent.velocity.magnitude * 2 / _agent.speed;
        _animator.SetFloat(MOVINGSPEED, speedValue);

    }

    private void Return()
    {
        _agent.SetDestination(_startPosition);

        float speedValue = _agent.velocity.magnitude / _agent.speed;
        _animator.SetFloat(MOVINGSPEED, speedValue);


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

                    _animator.SetTrigger(ISATTACKING);
                }
            }
        }
    }

    private void Die()
    {
        _animator.SetBool(ENEMYDEATH, true);
        _agent.enabled = false;
        this.enabled = false;

        StartCoroutine(DelayedDisable());
    }

    private IEnumerator DelayedDisable()
    {
        yield return new WaitForSeconds(3f); // 애니메이션 재생 시간
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

        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Zombie Reaction Hit"))
        {
            _animator.SetTrigger(HITREACTION);
        }
    }

    private void GenerateNextPatrolPoint()
    {
        Vector2 randomCircle = Random.insideUnitCircle * _patrolRadius;
        Vector3 rayOrigin = transform.position + new Vector3(randomCircle.x, _raycastHeightOffset, randomCircle.y);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, _raycastMaxDistance, _groundLayer))
        {
            _patrolPoints = new Vector3[1];
            _patrolPoints[0] = hit.point;
            _currentPatrolIndex = 0;
        }
        else Debug.LogWarning($"{name} 순찰 위치 생성 실패");
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