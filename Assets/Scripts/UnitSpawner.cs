using UnityEngine;
using System.Collections.Generic;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private GameObject allyPrefab1; // 普通の召喚
    [SerializeField] private GameObject allyPrefab2; // 半円配置
    [SerializeField] private Transform SpawnPoint;

    private List<GameObject> spawnedAllies2 = new List<GameObject>(); // 半円用

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (allyPrefab1 != null && SpawnPoint != null)
                Instantiate(allyPrefab1, SpawnPoint.position, Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnHalfCircleAlly(allyPrefab2);
        }
    }

    void SpawnHalfCircleAlly(GameObject prefab)
    {
        if (prefab != null && SpawnPoint != null)
        {
            GameObject newAlly = Instantiate(prefab, SpawnPoint.position, Quaternion.identity);
            spawnedAllies2.Add(newAlly);

            // 全ユニットの index / totalAllies を更新
            int total = spawnedAllies2.Count;
            for (int i = 0; i < spawnedAllies2.Count; i++)
            {
                DefenseAlly script = spawnedAllies2[i].GetComponent<DefenseAlly>();
                if (script != null)
                {
                    script.index = i;
                    script.totalAllies = total;
                }
            }
        }
    }
}
