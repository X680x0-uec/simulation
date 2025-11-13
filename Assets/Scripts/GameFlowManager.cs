using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] private MikoshiControllerImada mikoshi;   // 神輿
    [SerializeField] private ChoiceUI choiceUI;                 // UI
    [SerializeField] private Transform[] stageStartPoints;      // ステージ開始位置たち

    private int currentStage = 0;

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
            choiceUI.ShowChoices(this); // ← 1引数版
    }

    // --- ChoiceUI から呼ばれる：ボタン押下時 ---
    public void OnChoiceSelected(int index)
    {
        // UI非表示＆時間再開
        if (choiceUI != null)
            choiceUI.Hide();
        Time.timeScale = 1f;

        if (index == 0)
        {
            // 次のステージへ
            currentStage = (currentStage + 1) % stageStartPoints.Length;

            // 敵味方削除
            ClearAllUnits();

            // 神輿を移動＆再開
            mikoshi.transform.position = stageStartPoints[currentStage].position;
            mikoshi.BeginMove();

            Debug.Log($"ステージ {currentStage + 1} に移動");
        }
        else if (index == 1)
        {
            // リトライ（現在ステージ再スタート）
            mikoshi.transform.position = stageStartPoints[currentStage].position;
            mikoshi.BeginMove();
        }
        else
        {
            Debug.Log("ゲーム終了");
        }
    }

    // --- 敵と味方を削除する処理（ステージ切り替え時に使用） ---
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
