using System.Collections.Generic;
using UnityEngine;

public class AllyManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> AllyPrefabs;
    [Header("Initial HP per ally type (0=Attacker,1=Defencer,2=Archer)")]
    [SerializeField] private int[] initialHPs = new int[3] { 50, 50, 50 };
    [SerializeField] private Transform spawnPoint;
    // 味方ユニットの出現数を記録する配列
    static public int[] NumSpawn= {0, 0, 0};
    private Vector3 spawnPosition;
    // List of currently spawned ally units in spawn order (for debug P-key damage)
    private readonly List<AllyUnit> spawnedAllies = new List<AllyUnit>();
    [Header("Debug: damage to apply to allies when pressing P")]
    [SerializeField] private int debugDamageOnP = 10;
    
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
                    if (go != null)
                    {
                        Debug.Log("Ally 1 Spawned (paid)");
                        NumSpawn[0] = NumSpawn[0] + 1;
                        // Ensure the spawned object has an AllyUnit and initialize HP/type
                        var unit = go.GetComponent<AllyUnit>();
                        if (unit == null) unit = go.AddComponent<AllyUnit>();
                        int hp = (initialHPs != null && initialHPs.Length > 0) ? initialHPs[0] : 50;
                        unit.Initialize(hp, 0);
                        unit.OwnerManager = this;
                        spawnedAllies.Add(unit);
                    }
                    else Debug.Log("Ally 1 spawn failed: not enough points");
                }
                else
                {
                    var inst = Instantiate(AllyPrefabs[0], spawnPosition, Quaternion.identity);
                    NumSpawn[0] = NumSpawn[0] + 1;
                    // Initialize HP
                    var unit = inst.GetComponent<AllyUnit>();
                    if (unit == null) unit = inst.AddComponent<AllyUnit>();
                    int hp = (initialHPs != null && initialHPs.Length > 0) ? initialHPs[0] : 50;
                    unit.Initialize(hp, 0);
                    unit.OwnerManager = this;
                    spawnedAllies.Add(unit);
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
                    if (go != null)
                    {
                        Debug.Log("Ally 2 Spawned (paid)");
                        NumSpawn[1] = NumSpawn[1] + 1;
                        var unit = go.GetComponent<AllyUnit>();
                        if (unit == null) unit = go.AddComponent<AllyUnit>();
                        int hp = (initialHPs != null && initialHPs.Length > 1) ? initialHPs[1] : 50;
                        unit.Initialize(hp, 1);
                        unit.OwnerManager = this;
                        spawnedAllies.Add(unit);
                    }
                    else Debug.Log("Ally 2 spawn failed: not enough points");
                }
                else
                {
                    var inst = Instantiate(AllyPrefabs[1], spawnPosition, Quaternion.identity);
                    NumSpawn[1] = NumSpawn[1] + 1;
                    var unit = inst.GetComponent<AllyUnit>();
                    if (unit == null) unit = inst.AddComponent<AllyUnit>();
                    int hp = (initialHPs != null && initialHPs.Length > 1) ? initialHPs[1] : 50;
                    unit.Initialize(hp, 1);
                    unit.OwnerManager = this;
                    spawnedAllies.Add(unit);
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
                    if (go != null)
                    {
                        Debug.Log("Ally 3 Spawned (paid)");
                        NumSpawn[2] = NumSpawn[2] + 1;
                        var unit = go.GetComponent<AllyUnit>();
                        if (unit == null) unit = go.AddComponent<AllyUnit>();
                        int hp = (initialHPs != null && initialHPs.Length > 2) ? initialHPs[2] : 50;
                        unit.Initialize(hp, 2);
                        unit.OwnerManager = this;
                        spawnedAllies.Add(unit);
                    }
                    else Debug.Log("Ally 3 spawn failed: not enough points");
                }
                else
                {
                    var inst = Instantiate(AllyPrefabs[2], spawnPosition, Quaternion.identity);
                    NumSpawn[2] = NumSpawn[2] + 1;
                    var unit = inst.GetComponent<AllyUnit>();
                    if (unit == null) unit = inst.AddComponent<AllyUnit>();
                    int hp = (initialHPs != null && initialHPs.Length > 2) ? initialHPs[2] : 50;
                    unit.Initialize(hp, 2);
                    unit.OwnerManager = this;
                    spawnedAllies.Add(unit);
                    Debug.Log("Ally 3 Spawned (no CostManager)");
                }
            }
        }
        // Debug damage: press P to apply configured damage to allies in spawn order
        if (Input.GetKeyDown(KeyCode.P))
        {
            ApplyDebugDamageToAllies(debugDamageOnP);
        }
    }

    // Called by AllyUnit when it dies to remove from our list
    public void OnAllyRemoved(AllyUnit unit)
    {
        if (unit == null) return;
        spawnedAllies.Remove(unit);
    }

    private void ApplyDebugDamageToAllies(int damage)
    {
        if (spawnedAllies.Count == 0) return;

        // Apply damage in the order they were spawned. If an ally is null (destroyed), skip.
        for (int i = 0; i < spawnedAllies.Count; i++)
        {
            var ally = spawnedAllies[i];
            if (ally == null) continue;
            ally.TakeDamage(damage);
            // Only apply to the first (earliest spawned) ally and then stop.
            break;
        }
        // Clean up any null entries at the front
        while (spawnedAllies.Count > 0 && spawnedAllies[0] == null)
        {
            spawnedAllies.RemoveAt(0);
        }
    }
}