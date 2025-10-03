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
                Instantiate(AllyPrefabs[0], spawnPosition, Quaternion.identity);
                Debug.Log("Ally 1 Spawned");
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (AllyPrefabs[1] != null)
            {
                Instantiate(AllyPrefabs[1], spawnPosition, Quaternion.identity);
                Debug.Log("Ally 2 Spawned");
            }
        }
    }
}