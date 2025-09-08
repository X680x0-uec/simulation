using UnityEngine;

public class UnitSpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject allyPrefab;
    private Transform SpawnPoint;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnAlly();
        }
    }

    void SpawnAlly()
    {
        if (allyPrefab != null && SpawnPoint != null)
        {
            Instantiate(allyPrefab, SpawnPoint.position, Quaternion.identity);
        }
    }
}
