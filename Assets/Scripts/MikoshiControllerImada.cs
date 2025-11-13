using System;
using UnityEngine;

public class MikoshiControllerImada : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float reachThreshold = 0.1f;
    [SerializeField] private int maxHP = 100;

    private int currentHP;
    private int currentIndex;
    private bool isMoving = true;
    private Rigidbody2D rb;

    // --- ゴール到達イベント（UIへ通知用） ---
    public static event Action OnMikoshiReachedGoal;

    // 終点到達フラグ
    public bool hasReachedEnd { get; private set; } = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;
        BeginMove();
    }

    void FixedUpdate()
    {
        if (!isMoving || hasReachedEnd) return;

        Vector2 targetPosition = GetTargetPosition();
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    public void BeginMove()
    {
        currentIndex = 0;
        isMoving = true;
        hasReachedEnd = false;
        transform.position = lineRenderer.GetPosition(currentIndex) + lineRenderer.transform.position;
        rb.linearVelocity = Vector2.zero;
    }

    private Vector2 GetTargetPosition()
    {
        int nextIndex = currentIndex + 1;

        if (lineRenderer.positionCount <= nextIndex)
        {
            // ゴール到達！
            isMoving = false;
            hasReachedEnd = true;
            rb.linearVelocity = Vector2.zero;

            Debug.Log("神輿がゴールしました");

            // --- ゲーム停止＆UIに通知 ---
            Time.timeScale = 0f;
            OnMikoshiReachedGoal?.Invoke();

            return lineRenderer.GetPosition(currentIndex) + lineRenderer.transform.position;
        }

        Vector2 targetPosition = lineRenderer.GetPosition(nextIndex) + lineRenderer.transform.position;

        if (Vector2.Distance(transform.position, targetPosition) < reachThreshold)
        {
            currentIndex++;
            return GetTargetPosition(); // 次のポイントを再帰的に取得
        }

        return targetPosition;
    }
}
