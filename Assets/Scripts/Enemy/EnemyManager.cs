using System.Collections;  
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyManager : MonoBehaviour
{
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
    }

    [Header("出現位置設定（グローバル）")]
    [Tooltip("出現最小距離")]
    [SerializeField] private float spawnMinRange = 5f;
    [Tooltip("出現最大距離")]
    [SerializeField] private float spawnMaxRange = 10f;
    // 角度範囲は固定で -30〜30 度（元の実装と同じ）

    [Header("敵設定 — WaveDatabase から参照")]
    [Tooltip("敵データベースへの参照（ウェーブで生成する敵の定義）")]
    [SerializeField] public WaveDatabase waveDatabase;
    
    [Header("敵パラメータ参照")]
    [Tooltip("敵のHP/ダメージを参照するデータベース")]
    [SerializeField] private EnemyDatabase enemyDatabase;
    
    [SerializeField] private WaveConfig[] waves = new WaveConfig[0];
    [Tooltip("最後のウェーブ後に最初へ戻す")]
    [SerializeField] private bool loopWaves = true;
    [Tooltip("開始ウェーブのインデックス")]
    [SerializeField] private int startWaveIndex = 0;

    [Header("出現中心")]
    [Tooltip("優先して使用する対象オブジェクト（未指定時は Mikoshi タグで検索）")]
    [SerializeField] private GameObject mikoshiObject;
    [Tooltip("手動で設定する出現中心（mikoshiObject が無い場合に使用）")]
    [SerializeField] private Vector3 manualSpawnCenter = Vector3.zero;
    [Tooltip("Mikoshi タグを優先して使用する場合は true")]
    [SerializeField] private bool preferMikoshiTag = true;

    private Coroutine spawnRoutine;

    void Start()
    {
        if (preferMikoshiTag && mikoshiObject == null)
        {
            mikoshiObject = GameObject.FindWithTag("Mikoshi");
        }

        if (waveDatabase == null || waveDatabase.allEnemies == null || waveDatabase.allEnemies.Count == 0)
        {
            Debug.LogWarning("EnemyManager: WaveDatabase が未設定または敵が0です。生成は行われません。", this);
            return;
        }

        if (waves == null || waves.Length == 0)
        {
            Debug.LogWarning("EnemyManager: waves 配列が空です。生成は行われません。", this);
            return;
        }

        StartSpawning();
    }

    public void StartSpawning()
    {
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = null;
    }

    private IEnumerator SpawnLoop()
    {
        int waveIndex = Mathf.Clamp(startWaveIndex, 0, waves.Length - 1);

        do
        {
            var wave = waves[waveIndex];
            Vector3 spawnCenter = GetSpawnCenter();

            // ウェーブ開始前インターバル（各ウェーブごとに設定可能）
            if (wave != null && wave.intervalBeforeWave > 0f)
                yield return new WaitForSeconds(Mathf.Max(0f, wave.intervalBeforeWave));

            int count = Mathf.Max(0, wave.enemyCount);
            float totalDuration = Mathf.Max(0f, wave.waveDuration);
            float spawnDelay = (count > 0 && totalDuration > 0f) ? totalDuration / count : 0f;

            if (count == 0)
            {
                // 敵がいない場合はウェーブ長だけ待機して次へ
                if (totalDuration > 0f) yield return new WaitForSeconds(totalDuration);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    var enemy = PickEnemyByWeight();
                    if (enemy != null && enemy.prefab != null)
                    {
                        Vector3 pos = CalculateSpawnPosition(spawnCenter);
                        GameObject enemyObj = Instantiate(enemy.prefab, pos, Quaternion.identity);
                        
                        // EnemyDatabase から対応するデータを検索して初期化
                        EnemyController controller = enemyObj.GetComponent<EnemyController>();
                        if (controller != null && enemyDatabase != null)
                        {
                            EnemyData data = enemyDatabase.allEnemies.Find(e => e.prefab == enemy.prefab);
                            if (data != null)
                            {
                                controller.InitializeFromData(data.health, data.damage);
                            }
                            else
                            {
                                Debug.LogWarning($"EnemyDatabase に {enemy.prefab.name} のデータが見つかりません");
                            }
                        }
                    }

                    if (spawnDelay > 0f)
                        yield return new WaitForSeconds(spawnDelay);
                    else
                        yield return null; // 少しフレームを返して瞬間負荷を緩和
                }
            }

            // ウェーブ間インターバル
            yield return new WaitForSeconds(Mathf.Max(0f, wave.intervalAfterWave));

            waveIndex++;
            if (waveIndex >= waves.Length)
            {
                if (loopWaves) waveIndex = 0;
                else break;
            }
        } while (true);

        spawnRoutine = null;
    }

    // weight（整数）に基づく敵の抽選（WaveDatabase から）
    private WaveData PickEnemyByWeight()
    {
        if (waveDatabase == null || waveDatabase.allEnemies == null || waveDatabase.allEnemies.Count == 0)
            return null;

        int totalWeight = 0;
        foreach (var e in waveDatabase.allEnemies)
            totalWeight += (e != null) ? Mathf.Max(0, e.weight) : 0;

        if (totalWeight <= 0)
        {
            // 全て 0 の場合は均等選択
            int idx = Random.Range(0, waveDatabase.allEnemies.Count);
            return waveDatabase.allEnemies[idx];
        }

        int r = Random.Range(0, totalWeight);
        int acc = 0;
        foreach (var e in waveDatabase.allEnemies)
        {
            if (e == null) continue;
            acc += Mathf.Max(0, e.weight);
            if (r < acc) return e;
        }

        return waveDatabase.allEnemies[waveDatabase.allEnemies.Count - 1];
    }

    private Vector3 GetSpawnCenter()
    {
        if (preferMikoshiTag && mikoshiObject != null) return mikoshiObject.transform.position;
        if (mikoshiObject != null) return mikoshiObject.transform.position;
        return manualSpawnCenter;
    }

    // 元の実装に合わせた出現位置計算（角度 -30〜30 度、距離は spawnMinRange〜spawnMaxRange）
    private Vector3 CalculateSpawnPosition(Vector3 center)
    {
        float spawnDirection = Random.Range(-30f, 30f) * Mathf.Deg2Rad;
        float radius = Random.Range(Mathf.Max(0f, spawnMinRange), Mathf.Max(0f, spawnMaxRange));
        Vector2 offset2D = new Vector2(Mathf.Cos(spawnDirection), Mathf.Sin(spawnDirection)) * radius;
        return new Vector3(center.x + offset2D.x, center.y + offset2D.y, center.z);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // ウェーブ設定を検証
        if (waves != null)
        {
            for (int i = 0; i < waves.Length; i++)
            {
                var w = waves[i];
                if (w == null) continue;
                w.enemyCount = Mathf.Max(0, w.enemyCount);
                w.waveDuration = Mathf.Max(0f, w.waveDuration);
                w.intervalBeforeWave = Mathf.Max(0f, w.intervalBeforeWave);
                w.intervalAfterWave = Mathf.Max(0f, w.intervalAfterWave);
            }
        }

        spawnMinRange = Mathf.Max(0f, spawnMinRange);
        spawnMaxRange = Mathf.Max(spawnMinRange, spawnMaxRange);
    }
#endif
}