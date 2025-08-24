using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnMinRange = 5f;
    [SerializeField] private float spawnMaxRange = 10f;
    [SerializeField] private float spawnInterval = 2f;
    private float spawnTimer = 0f;
    [SerializeField] private int spawnCount = 15;
    [SerializeField] private GameObject mikoshiObject;
    private Vector2 spawnCenter;
    void Start()
    {
        mikoshiObject = GameObject.FindWithTag("Mikoshi");
    }
    void Update()
    {
        spawnCenter = mikoshiObject.transform.position;
        if (Time.time - spawnTimer >= spawnInterval)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                SpawnEnemy();
            }
            spawnTimer = Time.time;
        }
        
    }

    void SpawnEnemy()
    {
        float spawnDirection = Random.Range(0, 360);
        Vector2 spawnPosition = spawnCenter + new Vector2(Mathf.Cos(spawnDirection), Mathf.Sin(spawnDirection)) * Random.Range(spawnMinRange, spawnMaxRange);
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
