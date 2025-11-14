using System;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] private MikoshiControllerImada mikoshi;   // 神輿
    [SerializeField] private ChoiceUI choiceUI;                 // UI
    [SerializeField] private Transform[] stageStartPoints;      // ステージ開始位置たち

    private int currentStage = 0;
    private bool isTransitioning = false; // ステージ遷移中フラグ

    private void Start()
    {
        // UIを開始時に非表示
        if (choiceUI != null)
            choiceUI.gameObject.SetActive(false);
        else
            Debug.LogError("GameFlowManager: choiceUI が割り当てられていません");

        if (mikoshi == null)
            Debug.LogError("GameFlowManager: mikoshi が割り当てられていません");

        if (stageStartPoints == null || stageStartPoints.Length == 0)
            Debug.LogError("GameFlowManager: stageStartPoints が割り当てられていません");

        // 神輿ゴール時のイベント登録
        MikoshiControllerImada.OnMikoshiReachedGoal += OnGoalReached;
    }

    private void OnDestroy()
    {
        MikoshiControllerImada.OnMikoshiReachedGoal -= OnGoalReached;
    }

    // --- ゴール時にマップ選択UIを表示 ---
    private void OnGoalReached()
    {
        Debug.Log("ゴール！マップ選択UIを表示");
        Time.timeScale = 0f;  // ゲーム停止

        // ランダム3択
        string[] options = { "stageA", "stageB", "stageC" };
        
        if (choiceUI != null)
            choiceUI.ShowChoices(options, OnChoiceSelected);
    }

    // --- ChoiceUI から呼ばれる：ボタン押下時 ---
    private void OnChoiceSelected(int index)
    {
        if (isTransitioning)
            return;

        isTransitioning = true;

        // UI非表示＆時間再開
        if (choiceUI != null)
            choiceUI.Hide();
        Time.timeScale = 1f;

        // 選択肢に対応した位置へ移動
        if (index >= 0 && index < stageStartPoints.Length)
        {
            currentStage = index;

            // 敵削除
            ClearEnemyUnits();

            // 神輿を移動＆再開
            if (mikoshi != null)
            {
                mikoshi.transform.position = stageStartPoints[currentStage].position;
                mikoshi.BeginMove();
            }

            Debug.Log($"ステージ {currentStage + 1} に移動しました");
        }

        isTransitioning = false;
    }

    // --- 敵を削除する処理 ---
    private void ClearEnemyUnits()
    {
        // 敵（Enemyタグ）をすべて削除
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        

        Debug.Log($"敵 {enemies.Length} 体を削除しました。");
    }
}