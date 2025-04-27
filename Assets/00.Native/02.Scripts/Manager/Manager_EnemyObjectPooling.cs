using UnityEngine;
using System.Collections.Generic;

public class Manager_EnemyObjectPooling : MonoBehaviour
{
    public static Manager_EnemyObjectPooling Instance { get; private set; }

    // requirements
    public GameObject enemyPrefab;
    public int poolSize = 20;

    private List<GameObject> enemyPool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        CreatePool();
    }

    private void CreatePool()
    {
        enemyPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewEnemy();
        }
    }

    private void CreateNewEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform);
        enemy.SetActive(false);
        enemyPool.Add(enemy);
    }

    public GameObject FindDisabledEnemy()
    {
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                enemy.SetActive(true);
                return enemy;
            }
        }

        // if enemy is exeeded, create a new one
        CreateNewEnemy();
        GameObject newEnemy = enemyPool[enemyPool.Count - 1];
        newEnemy.SetActive(true);
        return newEnemy;
    }

    // 적을 풀로 반환
    public void ReturnEnemy(GameObject enemy)
    {
        if (enemy != null)
        {
            enemy.SetActive(false);
            enemy.transform.SetParent(transform);
            enemy.transform.localPosition = Vector3.zero;
            enemy.transform.localRotation = Quaternion.identity;
        }
    }
}
