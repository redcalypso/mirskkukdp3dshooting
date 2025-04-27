using UnityEngine;
using System.Collections;

public class Manager_EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private int _spawnCount = 5;
    [SerializeField] private float _spawnRadius = 10f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _raycastHeightOffset = 10f;
    [SerializeField] private float _raycastMaxDistance = 20f;
    [SerializeField] private int _maxSpawnAttempts = 3;

    [Header("Wave Settings")]
    [SerializeField] private float _spawnInterval = 10f;
    [SerializeField] private int _maxEnemies = 20;
    [SerializeField] private bool _autoStart = true;

    private float _nextSpawnTime;
    private int _currentEnemyCount;
    private bool _isSpawning;
    private const int MAXTOTALATTEMPTS = 100;

    private void Start()
    {
        if (_autoStart && ValidateLog())
        {
            if (Manager_EnemyObjectPooling.Instance == null)
            {
                GameObject poolObj = new GameObject("Manager_EnemyObjectPooling");
                Manager_EnemyObjectPooling pool = poolObj.AddComponent<Manager_EnemyObjectPooling>();
                pool.enemyPrefab = _enemyPrefab;
                pool.poolSize = _maxEnemies;
            }
            else
            {
                Manager_EnemyObjectPooling.Instance.enemyPrefab = _enemyPrefab;
                Manager_EnemyObjectPooling.Instance.poolSize = _maxEnemies;
            }

            StartSpawning();
        }
    }

    private void Update()
    {
        if (!_isSpawning) return;

        if (Time.time >= _nextSpawnTime && _currentEnemyCount < _maxEnemies)
        {
            SpawnWave();
            _nextSpawnTime = Time.time + _spawnInterval;
        }
    }

    public void StartSpawning()
    {
        if (!ValidateLog()) return;
        
        _isSpawning = true;
        _nextSpawnTime = Time.time;
    }

    public void StopSpawning()
    {
        _isSpawning = false;
    }

    

    private void SpawnWave()
    {
        int remainingSlots = _maxEnemies - _currentEnemyCount;
        int spawnAmount = Mathf.Min(_spawnCount, remainingSlots);
        
        int successfulSpawns = 0;
        int totalAttempts = 0;

        while (successfulSpawns < spawnAmount && totalAttempts < MAXTOTALATTEMPTS)
        {
            if (TrySpawnEnemy())
            {
                successfulSpawns++;
                _currentEnemyCount++;
            }
            totalAttempts++;
        }
    }

    private bool TrySpawnEnemy()
    {
        for (int attempt = 0; attempt < _maxSpawnAttempts; attempt++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;
            Vector3 randomPoint = transform.position + new Vector3(randomCircle.x, _raycastHeightOffset, randomCircle.y);

            if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, _raycastMaxDistance, _groundLayer))
            {
                GameObject enemy = Manager_EnemyObjectPooling.Instance.FindDisabledEnemy();
                if (enemy == null) return false;

                enemy.transform.position = hit.point;
                enemy.transform.up = hit.normal;
                enemy.transform.Rotate(0, Random.Range(0, 360), 0, Space.Self);

                var enemyComponent = enemy.GetComponent<Enemy>();
                if (enemyComponent != null) StartCoroutine(WatchEnemyDestruction(enemy));
                return true;
            }
        }
        return false;
    }

    private IEnumerator WatchEnemyDestruction(GameObject enemy)
    {
        yield return new WaitUntil(() => enemy == null);
        _currentEnemyCount--;
    }

    /// <summary>
    /// For Editor Mode: Draw spawn area and raycast height
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Draw spawn area
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);

        // Draw raycast height
        Gizmos.color = Color.green;
        Vector3 heightOffset = Vector3.up * _raycastHeightOffset;
        Gizmos.DrawLine(transform.position, transform.position + heightOffset);
        Gizmos.DrawWireSphere(transform.position + heightOffset, 0.5f);
    }

    private bool ValidateLog()
    {
        if (_enemyPrefab == null)
        {
            Debug.LogError($"[{GetType().Name}] Enemy prefab is not assigned");
            return false;
        }

        if (_spawnCount <= 0)
        {
            Debug.LogError($"[{GetType().Name}] Spawn count must be greater than 0");
            return false;
        }

        if (_groundLayer == 0)
        {
            Debug.LogWarning($"[{GetType().Name}] Ground layer is not set");
            return false;
        }

        return true;
    }
} 