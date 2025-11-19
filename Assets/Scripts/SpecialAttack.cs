using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class SpecialAttack : MonoBehaviour
{
    private bool usedSpecial = false; // 必殺技を使ったかどうか（1度きり）
    [Header("Special / HitStop")]
    [SerializeField] private bool enableHitStop = true;
    [SerializeField] private float hitStopDuration = 0.15f; // 秒（real time）
    [SerializeField][Range(0.0f, 1.0f)] private float hitStopTimeScale = 0.02f; // 小さくするほど止まって見える

    InputAction specialAction;

    void Awake()
    {
        usedSpecial = false;
        specialAction = InputSystem.actions.FindAction("Special");
        specialAction.performed += ctx => OnSpecialPerformed();
        specialAction.Enable();
    }

    void OnSpecialPerformed()
    {
        if (usedSpecial) return;

        const int specialCost = 30;
        if (CostManager.Instance != null)
        {
            if (CostManager.Instance.TrySpend(specialCost))
            {
                usedSpecial = true;
                // ヒットストップ演出を挟んでから敵を消す
                if (enableHitStop)
                {
                    StartCoroutine(PerformHitStopAndClear());
                }
                else
                {
                    ClearAllEnemies();
                }
                Debug.Log("Mikoshi special used: Cleared all enemies (or will clear after hit-stop).");
            }
            else
            {
                Debug.Log("Not enough points for Mikoshi special.");
            }
        }
        else
        {
            // CostManagerが無い場合は動作させない
            Debug.LogWarning("CostManager not found - cannot use special");
        }
    }

    private void ClearAllEnemies()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int count = 0;
        foreach (var e in enemies)
        {
            Destroy(e);
            count++;
        }
        Debug.Log($"ClearAllEnemies: destroyed {count} enemies.");
        GameStats.IncrementEnemyKill(count);
    }

    private IEnumerator PerformHitStopAndClear()
    {
        // 現在の timeScale / fixedDeltaTime を保存
        float prevTimeScale = Time.timeScale;
        float prevFixedDelta = Time.fixedDeltaTime;

        // timeScale を落とす（ほぼ停止）
        Time.timeScale = Mathf.Clamp(hitStopTimeScale, 0.0001f, 1f);
        Time.fixedDeltaTime = prevFixedDelta * Time.timeScale;

        // 実時間で待つ
        yield return new WaitForSecondsRealtime(hitStopDuration);

        // 復帰
        Time.timeScale = prevTimeScale;
        Time.fixedDeltaTime = prevFixedDelta;

        // 時間が戻ったら敵を消す
        ClearAllEnemies();
        Debug.Log("PerformHitStopAndClear: hit-stop finished and enemies cleared.");
    }
}
