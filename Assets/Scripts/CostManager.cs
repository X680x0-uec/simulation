using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PrefabCost
{
    public GameObject prefab;
    public int cost;
}

public class CostManager : MonoBehaviour
{
    public static CostManager Instance { get; private set; }

    [Header("ログ設定")]
    public bool enableLogs = true;
    [Header("コスト設定（プレハブごと）")]
    public List<PrefabCost> prefabCosts = new List<PrefabCost>();

    [Header("ポイント設定")]
    public int initialPoints = 100;
    public int maxPoints = 9999;

    private Dictionary<GameObject, int> costLookup = new Dictionary<GameObject, int>();
    private int currentPoints;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        currentPoints = initialPoints;
        // List -> Dictionary
        costLookup.Clear();
        foreach (var pc in prefabCosts)
        {
            if (pc.prefab == null) continue;
            if (!costLookup.ContainsKey(pc.prefab)) costLookup.Add(pc.prefab, pc.cost);
        }
    if (enableLogs) Debug.Log($"CostManager initialized. Points={currentPoints}, entries={costLookup.Count}");
    }

    /// <summary>
    /// 指定プレハブのコストを返す。未登録なら-1を返す。
    /// </summary>
    public int GetCost(GameObject prefab)
    {
        if (prefab == null) return -1;
        if (costLookup.TryGetValue(prefab, out int c)) return c;
        return -1;
    }

    public bool CanAfford(int cost)
    {
        return currentPoints >= cost;
    }

    public bool CanAfford(GameObject prefab)
    {
        int c = GetCost(prefab);
        if (c < 0) return false;
        return CanAfford(c);
    }

    /// <summary>
    /// 指定金額を支払う。支払えれば true を返す。
    /// </summary>
    public bool TrySpend(int cost)
    {
        if (!CanAfford(cost)) return false;
        currentPoints -= cost;
    if (enableLogs) Debug.Log($"CostManager: Spent {cost}. Remaining={currentPoints}");
        return true;
    }

    public bool TrySpend(GameObject prefab)
    {
        int c = GetCost(prefab);
        if (c < 0) return false;
        return TrySpend(c);
    }

    public void Refund(int cost)
    {
        currentPoints += cost;
        if (currentPoints > maxPoints) currentPoints = maxPoints;
    if (enableLogs) Debug.Log($"CostManager: Refunded {cost}. Current={currentPoints}");
    }

    public void Refund(GameObject prefab)
    {
        int c = GetCost(prefab);
        if (c < 0) return;
        Refund(c);
    }

    public void AddPoints(int amount)
    {
        currentPoints += amount;
        if (currentPoints > maxPoints) currentPoints = maxPoints;
    if (enableLogs) Debug.Log($"CostManager: Added {amount}. Current={currentPoints}");
    }

    public int GetCurrentPoints()
    {
        return currentPoints;
    }

    /// <summary>
    /// 指定プレハブのコストを支払ってInstantiateする。支払えなければ null を返す。
    /// </summary>
    public GameObject TrySpendAndSpawn(GameObject prefab, Vector2 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            if (enableLogs) Debug.LogWarning("TrySpendAndSpawn: prefab is null");
            return null;
        }
        int c = GetCost(prefab);
        if (c < 0)
        {
            if (enableLogs) Debug.LogWarning($"TrySpendAndSpawn: prefab {prefab.name} has no cost entry");
            return null;
        }
        if (!TrySpend(c))
        {
            if (enableLogs) Debug.Log($"TrySpendAndSpawn: cannot afford {prefab.name} cost={c}. Current={currentPoints}");
            return null;
        }
        var go = Instantiate(prefab, position, rotation);
        if (enableLogs) Debug.Log($"TrySpendAndSpawn: Spawned {prefab.name} at {position} cost={c}");
        return go;
    }
}
