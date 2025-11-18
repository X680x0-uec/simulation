using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("シーン設定")]
    [SerializeField] private string titleSceneName = "Title";

    void Update()
    {
        // Enter または Return キーが押されたら Title シーンへ
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            LoadTitleScene();
        }
    }

    private void LoadTitleScene()
    {
        Debug.Log("Loading Title Scene...");
        
        // Time.timeScale をリセット
        Time.timeScale = 1f;
        
        // ゲーム統計をリセット
        ResetGameStats();
        
        // Title シーンを読み込む
        SceneManager.LoadScene(titleSceneName);
    }

    private void ResetGameStats()
    {
        // GameStats をリセット
        try
        {
            GameStats.ResetAll();
            Debug.Log("GameStats reset successful");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"GameStats reset failed: {ex.Message}");
        }

        // AllyManager の召喚数をリセット
        try
        {
            if (AllyManager.NumSpawn != null)
            {
                for (int i = 0; i < AllyManager.NumSpawn.Length; i++)
                {
                    AllyManager.NumSpawn[i] = 0;
                }
                Debug.Log("AllyManager.NumSpawn reset successful");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"AllyManager.NumSpawn reset failed: {ex.Message}");
        }

        // CostManager をリセット（存在する場合）
        try
        {
            if (CostManager.Instance != null)
            {
                // CostManager に ResetPoints() メソッドがあれば呼び出す
                var resetMethod = CostManager.Instance.GetType().GetMethod("ResetPoints");
                if (resetMethod != null)
                {
                    resetMethod.Invoke(CostManager.Instance, null);
                    Debug.Log("CostManager reset successful");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"CostManager reset failed: {ex.Message}");
        }
    }

    // ボタンからも呼び出せるように public メソッドも用意
    public void OnReturnToTitleButtonClicked()
    {
        LoadTitleScene();
    }
}