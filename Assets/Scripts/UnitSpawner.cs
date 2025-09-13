using UnityEngine;

public class UnitSpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject allyPrefab1;

    [SerializeField]
    private GameObject allyPrefab2;

    [SerializeField]
    private Transform SpawnPoint;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1キー");
            SpawnAlly1();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("2キー");
            SpawnAlly2();
        }
    }

    void SpawnAlly1()
    {
        if (allyPrefab1 != null && SpawnPoint != null)
        {
            Instantiate(allyPrefab1, SpawnPoint.position, Quaternion.identity);
        }
    }

    void SpawnAlly2()
    {
        if (allyPrefab2 != null && SpawnPoint != null)
        {
            Instantiate(allyPrefab2, SpawnPoint.position, Quaternion.identity);
        }
    }
}
