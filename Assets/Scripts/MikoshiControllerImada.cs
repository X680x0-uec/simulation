using System;
using System.Collections;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MikoshiControllerImada : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float maxSpeed = 5f; // 最高速度を制限して暴れを抑える
    [SerializeField] private float minForceDistance = 0.1f; // 距離に対する力の下限
    [SerializeField] private float maxForceDistance = 1f; // 距離に対する力の上限（正規化前のスケール）
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float reachThreshold = 0.1f;
    [SerializeField] private int maxHP = 100;

    private int currentHP;

    // Expose read-only properties for UI and other systems
    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    private int currentIndex;
    private bool isMoving = true;

    public static event Action OnMikoshiReachedGoal;

    // 終点に到達したかどうかのフラグ
    public bool hasReachedEnd { get; private set; } = false;

    private Rigidbody2D rb;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;
        BeginMove();
    }
    void Update()
    {
    if (isMoving)
        {
            Vector2 targetPosition = GetTargetPosition();
            Vector2 direction = targetPosition - (Vector2)transform.position;

            // 距離に応じて力の大きさを滑らかに決める（過剰な力を与えない）
            float distance = direction.magnitude;
            if (distance > reachThreshold)
            {
                Vector2 dir = direction.normalized;
                // distance を [minForceDistance, maxForceDistance] にクランプして力に反映
                float forceScale = Mathf.Clamp(distance, minForceDistance, maxForceDistance);
                float force = speed * Mathf.Min(forceScale, 1f);
                rb.AddForce(dir * force);
            }
            else
            {
                // 目的地に近ければ速度を止めて振動を抑える
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }

            // 速度が大きくなりすぎたら制限する
            if (rb != null && rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
            // Debug.Log("神輿が移動中");
        }

    }

    public void BeginMove()
    {
        currentIndex = 0;
        isMoving = true;
        hasReachedEnd = false; // ← リセット
        transform.position = lineRenderer.GetPosition(currentIndex) + lineRenderer.transform.position;
        // 前回の勢いをリセット（シーン遷移やリトライ後の暴れ防止）
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    private Vector2 GetTargetPosition()
    {
        int nextIndex = currentIndex + 1;

        if (lineRenderer.positionCount <= nextIndex)
        {
            isMoving = false;
            hasReachedEnd = true;  // ← 終点に到達したらフラグON
            Debug.Log("神輿がゴールしました");

            // 停止させてからイベント通知（残った速度で飛んでいかないように）
            if (rb != null) rb.linearVelocity = Vector2.zero;
            Time.timeScale = 0f;
            OnMikoshiReachedGoal?.Invoke();
            
            return lineRenderer.GetPosition(currentIndex) + lineRenderer.transform.position;
        }

        Vector2 targetPosition = lineRenderer.GetPosition(nextIndex) + lineRenderer.transform.position;
        if (Vector2.Distance(transform.position, targetPosition) < reachThreshold)
        {
            currentIndex++;
            return GetTargetPosition();
        }
        return targetPosition;
    }
}