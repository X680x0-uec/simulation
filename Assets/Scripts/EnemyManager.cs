using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnMinRange = 5f;
    [SerializeField] private float spawnMaxRange = 10f;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int spawnCount = 15;
    private GameObject mikoshiObject;
    private Vector2 mikoshiPosition;
    void Start()
    {
        mikoshiObject = GameObject.FindWithTag("Player");
    }
    void Update()
    {
        mikoshiPosition = mikoshiObject.GetComponent<Transform>().position;
        for (int i = 0; i < spawnCount; i++)
        {
            //SpawnEnemy();
            //yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        float spawnDirection = Random.Range(0, 360);
        Vector2 spawnPosition = mikoshiPosition + new Vector2(Mathf.Cos(spawnDirection), Mathf.Sin(spawnDirection)) * Random.Range(spawnMinRange, spawnMaxRange);
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
