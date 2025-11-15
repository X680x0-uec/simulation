using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemyData
{
    public string name;
    public GameObject prefab;
    public int health;
    public int cost;
    public Sprite icon;
    
    [Header("出現割合（相対重み）")]
    [Min(0)]
    public int weight = 1; // 敵の出現確率を制御するための重み

    [Header("正規化確率（表示）")]
    [Tooltip("OnValidate で計算される確率（0〜1）。編集しないでください。")]
    [SerializeField] private float normalizedProbability = 0f;

    public float Probability => normalizedProbability;
    internal void SetNormalizedProbability(float p) => normalizedProbability = p;
}

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Scriptable Objects/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    public List<EnemyData> allEnemies = new List<EnemyData>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 敵の重みから表示用確率を計算
        if (allEnemies != null && allEnemies.Count > 0)
        {
            int totalWeight = 0;
            for (int i = 0; i < allEnemies.Count; i++)
            {
                if (allEnemies[i] != null)
                {
                    allEnemies[i].weight = Mathf.Max(0, allEnemies[i].weight);
                    totalWeight += allEnemies[i].weight;
                }
            }

            if (totalWeight > 0)
            {
                for (int i = 0; i < allEnemies.Count; i++)
                {
                    if (allEnemies[i] != null)
                    {
                        allEnemies[i].SetNormalizedProbability((float)allEnemies[i].weight / totalWeight);
                    }
                }
            }
            else
            {
                int len = allEnemies.Count;
                for (int i = 0; i < len; i++)
                {
                    if (allEnemies[i] != null)
                    {
                        allEnemies[i].SetNormalizedProbability(len > 0 ? 1f / len : 0f);
                    }
                }
            }
        }
    }
#endif
}
