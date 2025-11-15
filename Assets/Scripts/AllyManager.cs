using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class AllyManager : MonoBehaviour
{
    public AllyDatabase allyDatabase;
    [SerializeField] private Transform spawnPoint;
    // 味方ユニットの出現数を記録する配列
    static public int[] NumSpawn = {0, 0, 0};
    private Vector3 spawnPosition;
    // List of currently spawned ally units in spawn order (for debug P-key damage)
    private readonly List<AllyUnit> spawnedAllies = new List<AllyUnit>();
    [Header("Debug: damage to apply to allies when pressing P")]
    [SerializeField] private int debugDamageOnP = 10;

    [SerializeField] private AllyIconManager allyIconManager;
    private InputAction spawnAction;
    private InputAction rightShift;
    private InputAction leftShift;
    public static int index;
    
    void Awake()
	{
		spawnAction = InputSystem.actions.FindAction("Spawn");
        rightShift = InputSystem.actions.FindAction("Right");
        leftShift = InputSystem.actions.FindAction("Left");
        index = 0;
        rightShift.performed += ctx =>
        {
            index = (index + 1) % allyDatabase.allAllies.Count;
            // Debug.Log("Selected Ally Index: " + index);
            allyIconManager.SwitchIcon(index, allyDatabase);
        };

        leftShift.performed += ctx =>
        {
            index = (index + allyDatabase.allAllies.Count - 1) % allyDatabase.allAllies.Count;
            // Debug.Log("Selected Ally Index: " + index);
            allyIconManager.SwitchIcon(index, allyDatabase);
        };
        spawnAction.performed += ctx => SpawnAlly();

        spawnAction.Enable();
        rightShift.Enable();
        leftShift.Enable();
	}

    void Update()
    {
        spawnPosition = spawnPoint.position;

        // Debug damage: press P to apply configured damage to allies in spawn order
        if (Input.GetKeyDown(KeyCode.P))
        {
            ApplyDebugDamageToAllies(debugDamageOnP);
        }
    }

    public void SpawnAlly()
    {
        AllyData ally = allyDatabase.allAllies[index];
        if (ally.prefab != null)
        {
            if (CostManager.Instance != null)
            {
                var go = CostManager.Instance.TrySpendAndSpawn(ally.prefab, spawnPosition, Quaternion.identity);
                if (go != null)
                {
                    Debug.Log(ally.name + " Spawned (paid)");
                    NumSpawn[0] = NumSpawn[0] + 1;
                    // Ensure the spawned object has an AllyUnit and initialize HP/type
                    var unit = go.GetComponent<AllyUnit>();
                    if (unit == null) unit = go.AddComponent<AllyUnit>();
                    int hp = ally.health;
                    unit.Initialize(hp, 0);
                    unit.OwnerManager = this;
                    spawnedAllies.Add(unit);
                }
                else Debug.Log(ally.prefab.name + " spawn failed: not enough points");
            }
            else
            {
                var inst = Instantiate(ally.prefab, spawnPosition, Quaternion.identity);
                NumSpawn[0] = NumSpawn[0] + 1;
                // Initialize HP
                var unit = inst.GetComponent<AllyUnit>();
                if (unit == null) unit = inst.AddComponent<AllyUnit>();
                int hp = ally.health;
                unit.Initialize(hp, 0);
                unit.OwnerManager = this;
                spawnedAllies.Add(unit);
                Debug.Log(ally.name + " Spawned (no CostManager)");
            }
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