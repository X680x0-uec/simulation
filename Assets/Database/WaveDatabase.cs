using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemyEntry
{
    public string name;
    public GameObject prefab;
    
    [Header("出現割合（相対重み）")]
    [Min(0)]
    public int weight = 1; // 敵の出現確率を制御するための重み

    [Header("正規化確率（表示）")]
    [Tooltip("OnValidate で計算される確率（0〜1）。編集しないでください。")]
    [SerializeField] private float normalizedProbability = 0f;

    public float Probability => normalizedProbability;
    internal void SetNormalizedProbability(float p) => normalizedProbability = p;
}

[System.Serializable]
public class WaveConfig
{
    [Tooltip("ウェーブ名")]
    public string waveName = "Wave";

    [Tooltip("このウェーブで召喚する敵の数（固定）")]
    [Min(0)]
    public int enemyCount = 10;

    [Tooltip("ウェーブの長さ（秒）。敵はこの時間内に等間隔で召喚されます。0 にすると同フレームで一括生成")]
    [Min(0f)]
    public float waveDuration = 0f;

    [Tooltip("ウェーブ開始前、ウェーブが始まるまでの待ち時間（秒）")]
    [Min(0f)]
    public float intervalBeforeWave = 0f;

    [Tooltip("ウェーブ終了後、次のウェーブが始まるまでの待ち時間（秒）")]
    [Min(0f)]
    public float intervalAfterWave = 2f;

    [Tooltip("このウェーブで出現する敵のリスト")]
    public List<EnemyEntry> enemyEntries = new List<EnemyEntry>();

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 敵の重みから表示用確率を計算
        if (enemyEntries != null && enemyEntries.Count > 0)
        {
            int totalWeight = 0;
            for (int i = 0; i < enemyEntries.Count; i++)
            {
                if (enemyEntries[i] != null)
                {
                    enemyEntries[i].weight = Mathf.Max(0, enemyEntries[i].weight);
                    totalWeight += enemyEntries[i].weight;
                }
            }

            if (totalWeight > 0)
            {
                for (int i = 0; i < enemyEntries.Count; i++)
                {
                    if (enemyEntries[i] != null)
                    {
                        enemyEntries[i].SetNormalizedProbability((float)enemyEntries[i].weight / totalWeight);
                    }
                }
            }
            else
            {
                int len = enemyEntries.Count;
                for (int i = 0; i < len; i++)
                {
                    if (enemyEntries[i] != null)
                    {
                        enemyEntries[i].SetNormalizedProbability(len > 0 ? 1f / len : 0f);
                    }
                }
            }
        }
    }
#endif
}

[CreateAssetMenu(fileName = "WaveDatabase", menuName = "Scriptable Objects/WaveDatabase")]
public class WaveDatabase : ScriptableObject
{
    public WaveConfig[] waves = new WaveConfig[0];
    [Tooltip("最後のウェーブ後に最初へ戻す")]
    public bool loopWaves = true;
    [Tooltip("開始ウェーブのインデックス")]
    public int startWaveIndex = 0;
}