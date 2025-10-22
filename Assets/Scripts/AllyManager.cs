using System.Collections.Generic;
using UnityEngine;

public class AllyManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> AllyPrefabs;
    [SerializeField] private Transform spawnPoint;

    private Vector3 spawnPosition;
    
    void Update()
    {
        spawnPosition = spawnPoint.position;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (AllyPrefabs[0] != null)
            {
                if (CostManager.Instance != null)
                {
                    var go = CostManager.Instance.TrySpendAndSpawn(AllyPrefabs[0], spawnPosition, Quaternion.identity);
                    if (go != null) Debug.Log("Ally 1 Spawned (paid)");
                    else Debug.Log("Ally 1 spawn failed: not enough points");
                }
                else
                {
                    Instantiate(AllyPrefabs[0], spawnPosition, Quaternion.identity);
                    Debug.Log("Ally 1 Spawned (no CostManager)");
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (AllyPrefabs[1] != null)
            {
                if (CostManager.Instance != null)
                {
                    var go = CostManager.Instance.TrySpendAndSpawn(AllyPrefabs[1], spawnPosition, Quaternion.identity);
                    if (go != null) Debug.Log("Ally 2 Spawned (paid)");
                    else Debug.Log("Ally 2 spawn failed: not enough points");
                }
                else
                {
                    Instantiate(AllyPrefabs[1], spawnPosition, Quaternion.identity);
                    Debug.Log("Ally 2 Spawned (no CostManager)");
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (AllyPrefabs[2] != null)
            {
                if (CostManager.Instance != null)
                {
                    var go = CostManager.Instance.TrySpendAndSpawn(AllyPrefabs[2], spawnPosition, Quaternion.identity);
                    if (go != null) Debug.Log("Ally 3 Spawned (paid)");
                    else Debug.Log("Ally 3 spawn failed: not enough points");
                }
                else
                {
                    Instantiate(AllyPrefabs[2], spawnPosition, Quaternion.identity);
                    Debug.Log("Ally 3 Spawned (no CostManager)");
                }
            }
        }
    }
}