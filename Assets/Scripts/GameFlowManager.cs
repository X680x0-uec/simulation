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

    private int currentStage = 0;
    private bool isTransitioning = false;
    private List<int> currentStageIndices = new List<int>(); // 現在表示中のステージインデックス

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
        Debug.Log("ゴール！マップ選択UIを表示");
        Time.timeScale = 0f;

        // 4つのステージからランダムに3つを選択
        currentStageIndices = GetRandomStageIndices(3);
        
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

    // ランダムにステージインデックスを選択
    private List<int> GetRandomStageIndices(int count)
    {
        List<int> allIndices = new List<int>();
        for (int i = 0; i < stageStartPoints.Length; i++)
        {
            allIndices.Add(i);
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
        if (isTransitioning)
            return;

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
                // ステージに対応したラインを設定
                if (currentStage < stageLines.Length && stageLines[currentStage] != null)
                {
                    mikoshi.SetLineRenderer(stageLines[currentStage]);
                }
                else
                {
                    Debug.LogError($"ステージ {currentStage} のラインが設定されていません");
                }

                // 開始位置に移動
                mikoshi.transform.position = stageStartPoints[currentStage].position;
                mikoshi.BeginMove();
            }

            Debug.Log($"ステージ {stageNames[currentStage]} に移動しました");
        }

        isTransitioning = false;
    }

    private void ClearEnemyUnits()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        Debug.Log($"敵 {enemies.Length} 体を削除しました。");
    }
}