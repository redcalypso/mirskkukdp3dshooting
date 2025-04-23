using UnityEngine;

public enum EnemyState
{
    Idle,
    Track,
    Return,
    Damaged,
    Attack,
    Die
}
public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyState _enemyState;
    [SerializeField] private Transform _player;

    private CharacterController _characterController;
    public float _speed = 5f;
    public float _findDistance = 10f;
    public float _attackDistance = 2f;
    private Vector3 _startPosition;

    private void Start()
    {
        _player = GameObject.Find("Player").transform;
        _enemyState = EnemyState.Idle;
    }
    
    private void Update()
    {
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
            case EnemyState.Damaged:
                // Damaged();
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
        Debug.Log("Idle");
        if(Vector3.Distance(transform.position, _player.position) < _findDistance)
        {
            _enemyState = EnemyState.Track;
        }
    }

    private void Track()
    {
        Debug.Log("Track");
        Vector3 direction = (_player.position - transform.position).normalized;
        _characterController.Move(direction * _speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, _player.position) > _findDistance)
        {
            _enemyState = EnemyState.Return;
        }

        if(Vector3.Distance(transform.position, _player.position) < _attackDistance)
        {
            _enemyState = EnemyState.Attack;
        }
    }

    private void Return()
    {
        Debug.Log("Return");
        if(Vector3.Distance(transform.position, _startPosition) <= 0.1f)
        {
            Vector3 direction = (_startPosition - transform.position).normalized;
            _characterController.Move(direction * _speed * Time.deltaTime);
        }
        else
        {
            _enemyState = EnemyState.Idle;
        }
    }

    /*private void Damaged(Damage damage)
    {
        Debug.Log("Damaged");
        Health -= damage.Value;

        _enemyState = EnemyState.Track;
    }*/

    private void Attack()
    {
        Debug.Log("Attack");
        if(Vector3.Distance(transform.position, _player.position) > _attackDistance)
        {

        }
    }

    private void Die()
    {
        Debug.Log("Die");
    }
}
