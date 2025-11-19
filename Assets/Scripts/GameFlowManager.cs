using System;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] private MikoshiControllerImada mikoshi;   // 神輿
    [SerializeField] private ChoiceUI choiceUI;                 // UI
    [SerializeField] private Transform[] stageStartPoints;      // ステージ開始位置たち（4つ）
    [SerializeField] private LineRenderer[] stageLines;         // 各ステージのラインレンダラー（4つ）
    [SerializeField] private string[] stageNames = { "stageA", "stageB", "stageC", "stageD" }; // ステージ名

    [Header("味方転移設定")]
    [SerializeField] private float allyTeleportRadius = 3f;     // 味方を配置する半径

    private int currentStage = 0;
    private bool isTransitioning = false;
    private List<int> currentStageIndices = new List<int>(); // 現在表示中のステージインデックス
    private int goalCount = 0; // ゴール回数カウント

    private void Start()
    {
        if (choiceUI != null)
            choiceUI.gameObject.SetActive(false);
        else
            Debug.LogError("GameFlowManager: choiceUI が割り当てられていません");

        if (mikoshi == null)
            Debug.LogError("GameFlowManager: mikoshi が割り当てられていません");

        if (stageStartPoints == null || stageStartPoints.Length < 4)
            Debug.LogError("GameFlowManager: stageStartPoints は4つ必要です");

        if (stageLines == null || stageLines.Length < 4)
            Debug.LogError("GameFlowManager: stageLines は4つ必要です");

        MikoshiControllerImada.OnMikoshiReachedGoal += OnGoalReached;
    }

    private void OnDestroy()
    {
        MikoshiControllerImada.OnMikoshiReachedGoal -= OnGoalReached;
    }

    private void OnGoalReached()
    {
        goalCount++;
        Debug.Log($"ゴール！マップ選択UIを表示（{goalCount}回目のゴール）");
        Time.timeScale = 0f;

        if (goalCount == 1)
        {
            // 1回目：ステージ3（インデックス3）以外から3つ選択
            currentStageIndices = GetRandomStageIndicesExcluding(3, 3);
        }
        else if (goalCount == 2)
        {
            // 2回目：ステージ3（インデックス3）のみを表示
            currentStageIndices = new List<int> { 3 };
        }

        // 配列番号の小さい順にソート
        currentStageIndices.Sort();
        
        // 選択肢の名前を取得
        string[] options = new string[currentStageIndices.Count];
        for (int i = 0; i < currentStageIndices.Count; i++)
        {
            int stageIndex = currentStageIndices[i];
            options[i] = stageIndex < stageNames.Length ? stageNames[stageIndex] : $"Stage {stageIndex + 1}";
        }
        
        if (choiceUI != null)
        {
            choiceUI.gameObject.SetActive(true);
            choiceUI.ShowChoices(options, OnChoiceSelected);
        }

        Debug.Log($"表示されたステージ: {string.Join(", ", options)}");
    }

    // ランダムにステージインデックスを選択（特定のインデックスを除外）
    private List<int> GetRandomStageIndicesExcluding(int excludeIndex, int count)
    {
        List<int> allIndices = new List<int>();
        for (int i = 0; i < stageStartPoints.Length; i++)
        {
            if (i != excludeIndex) // 指定されたインデックスを除外
            {
                allIndices.Add(i);
            }
        }

        // シャッフル
        for (int i = 0; i < allIndices.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, allIndices.Count);
            int temp = allIndices[i];
            allIndices[i] = allIndices[randomIndex];
            allIndices[randomIndex] = temp;
        }

        // 最初の count 個を返す
        List<int> result = new List<int>();
        for (int i = 0; i < Mathf.Min(count, allIndices.Count); i++)
        {
            result.Add(allIndices[i]);
        }

        return result;
    }

    private void OnChoiceSelected(int index)
    {
        Debug.Log($"選択されたステージインデックス: {index}");
        if (isTransitioning) return;

        isTransitioning = true;

        if (choiceUI != null)
        {
            choiceUI.Hide();
            choiceUI.gameObject.SetActive(false);
        }
        
        Time.timeScale = 1f;

        // 選択されたボタンのインデックスから実際のステージインデックスを取得
        if (index >= 0 && index < currentStageIndices.Count)
        {
            currentStage = currentStageIndices[index];

            ClearEnemyUnits();

            if (mikoshi != null)
            {
                // 開始位置に移動
                Vector3 newMikoshiPosition = stageStartPoints[currentStage].position;
                mikoshi.transform.position = newMikoshiPosition;

                // 味方ユニットを神輿の周囲に転移
                TeleportAlliesToMikoshi(newMikoshiPosition);

                // ステージに対応したラインを設定
                if (currentStage < stageLines.Length && stageLines[currentStage] != null)
                {
                    mikoshi.SetLineRenderer(stageLines[currentStage]);
                }
                else
                {
                    Debug.LogError($"ステージ {currentStage} のラインが設定されていません");
                }

                mikoshi.BeginMove();
            }

            Debug.Log($"ステージ {stageNames[currentStage]} に移動しました");
        }

        isTransitioning = false;
    }

    // 味方ユニットを神輿の周囲に転移させる
    private void TeleportAlliesToMikoshi(Vector3 mikoshiPosition)
    {
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
        
        if (allies.Length == 0)
        {
            Debug.Log("転移させる味方ユニットがいません");
            return;
        }

        // 円形に配置
        float angleStep = 360f / allies.Length;
        
        for (int i = 0; i < allies.Length; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * allyTeleportRadius,
                Mathf.Sin(angle) * allyTeleportRadius,
                0
            );
            
            allies[i].transform.position = mikoshiPosition + offset;

            // Rigidbody2D の速度をリセット
            Rigidbody2D rb = allies[i].GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        Debug.Log($"味方 {allies.Length} 体を神輿の周囲に転移させました");
    }

    private void ClearEnemyUnits()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                // Collider を無効化してから削除
                Collider2D collider = enemy.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = false;
                }

                // スクリプトを無効化
                MonoBehaviour[] scripts = enemy.GetComponents<MonoBehaviour>();
                foreach (var script in scripts)
                {
                    if (script != null)
                        script.enabled = false;
                }

                Destroy(enemy);
            }
        }

        Debug.Log($"敵 {enemies.Length} 体を削除しました。");
    }
}