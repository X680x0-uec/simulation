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

        // 神輿ゴール時のイベント登録
        MikoshiControllerImada.OnMikoshiReachedGoal += ShowMapChoiceUI;
    }

    private void OnDestroy()
    {
        MikoshiControllerImada.OnMikoshiReachedGoal -= ShowMapChoiceUI;
    }

    // --- ゴール時にマップ選択UIを表示 ---
    private void ShowMapChoiceUI()
    {
        Debug.Log("ゴール！マップ選択UIを表示");
        Time.timeScale = 0f;  // ゲーム停止

        if (choiceUI != null)
            choiceUI.ShowChoices(this);
    }

    // --- ChoiceUI から呼ばれる：ボタン押下時 ---
    public void OnChoiceSelected(int index)
    {
        if (isTransitioning)
            return;

        isTransitioning = true;

        // UI非表示＆時間再開
        if (choiceUI != null)
            choiceUI.Hide();
        Time.timeScale = 1f;

        if (index == 0)
        {
            // 次のステージへ
            TransitionToNextStage();
        }
        else if (index == 1)
        {
            // リトライ（現在ステージ再スタート）
            RetryCurrentStage();
        }
        else
        {
            // ゲーム終了
            QuitGame();
        }

        isTransitioning = false;
    }

    // --- 次のステージへ遷移 ---
    private void TransitionToNextStage()
    {
        currentStage = (currentStage + 1) % stageStartPoints.Length;
        
        // 敵味方削除
        ClearAllUnits();

        // 神輿を移動＆再開
        if (mikoshi != null)
        {
            mikoshi.transform.position = stageStartPoints[currentStage].position;
            mikoshi.BeginMove();
        }

        Debug.Log($"ステージ {currentStage + 1} に移動しました");
    }

    // --- 現在のステージをリトライ ---
    private void RetryCurrentStage()
    {
        // 敵味方削除
        ClearAllUnits();

        // 神輿を再開
        if (mikoshi != null)
        {
            mikoshi.transform.position = stageStartPoints[currentStage].position;
            mikoshi.BeginMove();
        }

        Debug.Log($"ステージ {currentStage + 1} をリトライしました");
    }

    // --- ゲーム終了 ---
    private void QuitGame()
    {
        Debug.Log("ゲーム終了");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // --- 敵と味方を削除する処理 ---
    private void ClearAllUnits()
    {
        // 敵（Enemyタグ）をすべて削除
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        // 味方（Allyタグ）をすべて削除
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject ally in allies)
        {
            Destroy(ally);
        }

        Debug.Log($"敵 {enemies.Length} 体、味方 {allies.Length} 体を削除しました。");
    }
}