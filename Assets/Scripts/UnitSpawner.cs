using UnityEngine;

public class UnitSpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject allyPrefab;
    [SerializeField]
    private Transform SpawnPoint;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1キー");
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
