using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MikoshiControllerImada : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float minForceDistance = 0.1f;
    [SerializeField] private float maxForceDistance = 1f;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float reachThreshold = 0.1f;
    [SerializeField] private int maxHP = 100;

    [Header("現在の状態")]
    [SerializeField] private int currentHP; // Inspector に表示

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    private int currentIndex;
    private bool isMoving = true;

    public static event Action OnMikoshiReachedGoal;

    public bool hasReachedEnd { get; private set; } = false;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;
        BeginMove();
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Vector2 targetPosition = GetTargetPosition();
            Vector2 direction = targetPosition - (Vector2)transform.position;

            float distance = direction.magnitude;
            if (distance > reachThreshold)
            {
                Vector2 dir = direction.normalized;
                float forceScale = Mathf.Clamp(distance, minForceDistance, maxForceDistance);
                float force = speed * Mathf.Min(forceScale, 1f);
                rb.AddForce(dir * force);
            }

            // 速度制限
            if (rb != null && rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
    }

    /// <summary>
    /// Mikoshi にダメージを与える（外部から呼び出すための API）。
    /// attackerPosition を渡すと簡易ノックバックを行います。
    /// </summary>
    public void TakeDamage(int damage, Vector3 attackerPosition)
    {
        currentHP -= damage;
        Debug.Log($"Mikoshi took {damage} damage. Remaining HP: {currentHP}/{maxHP}");
        
        if (currentHP <= 0)
        {
            currentHP = 0;
            isMoving = false;
            Debug.Log("Mikoshi destroyed - Loading GameOverScene");
            
            // Time.timeScale をリセットしてからシーン遷移
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameOverScene");
        }
    }

    // 新しいラインを設定するメソッド
    public void SetLineRenderer(LineRenderer newLine)
    {
        if (newLine != null)
        {
            lineRenderer = newLine;
            Debug.Log($"新しいラインが設定されました: {newLine.name}");
        }
        else
        {
            Debug.LogError("SetLineRenderer: 無効な LineRenderer が渡されました");
        }
    }

    public void BeginMove()
    {
        currentIndex = 0;
        isMoving = true;
        hasReachedEnd = false;

        if (lineRenderer != null)
        {
            transform.position = lineRenderer.GetPosition(currentIndex) + lineRenderer.transform.position;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private Vector2 GetTargetPosition()
    {
        if (lineRenderer == null)
            return transform.position;

        int nextIndex = currentIndex + 1;

        if (lineRenderer.positionCount <= nextIndex)
        {
            isMoving = false;
            hasReachedEnd = true;
            Debug.Log("神輿がゴールしました");

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