using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DefenseAllyImada : MonoBehaviour
{
    //いまだ早く攻撃された時のやつを実装しろ

    [Header("半円配置")]
    [SerializeField] private Transform mikoshi;       // 自機（スクリプト参照）
    [SerializeField] private float radius = 3f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveThreshold = 0.1f;

    private static int totalAllies;
    private int index;

    [Header("攻撃設定")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private Renderer attackRangeIndicator;
    [SerializeField] private float attackRangeVisualizeDuration = 0.1f;
    [SerializeField] private int attackPower = 10;
    [SerializeField] private float attackCooldown = 1.0f;

    private List<Transform> enemiesInRange = new List<Transform>();
    private bool canAttack = true;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError("DefenseAlly に Rigidbody2D が必要じゅう！");
    }

    void Start()
    {
        if (mikoshi == null)
        {
            GameObject obj = GameObject.FindWithTag("Mikoshi");
            if (obj != null) mikoshi = obj.transform;
            else Debug.LogError("タグ: Mikoshi が見つからないじゅう！");
        }
    }

    void OnEnable()
    {
        totalAllies++;
        index = totalAllies - 1; // 0から始まるインデックス
    }

    void Update()
    {
        // 半円追従
        FollowMikoshi();

        // 範囲内の敵を検出
        DetectInRangeEnemies();
        // 攻撃
        if (canAttack && enemiesInRange.Count > 0)
        {
            StartCoroutine(AttackAllEnemies());
        }
    }

    void FollowMikoshi()
    {
        // ミコシに向かって移動
        if (mikoshi != null)
        {
            float angleStep = 180f / (totalAllies + 1);
            float angle = (-90f + angleStep * (index + 1)) * Mathf.Deg2Rad;
            Vector2 destination = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius + (Vector2)mikoshi.position;
            Vector2 direction = destination - (Vector2)transform.position;
            if (direction.magnitude > moveThreshold)
            {
                rb.AddForce(direction.normalized * moveSpeed);
            }
            else
            {
                rb.linearVelocity = Utils.GetVelocityOfGameObject2D(mikoshi.gameObject);
            }
        }
    }

    private void DetectInRangeEnemies()
    {
        enemiesInRange.Clear();
        enemiesInRange = Utils.FetchInRangeObjectsWithTag(transform, "Enemy", attackRange);
    }

    private IEnumerator AttackAllEnemies()
    {
        canAttack = false;

        // コピーを作成してループ
        var enemiesSnapshot = new List<Transform>(enemiesInRange);

        foreach (Transform enemy in enemiesSnapshot)
        {
            if (enemy != null)
            {
                EnemyController enemyScript = enemy.GetComponent<EnemyController>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(attackPower, transform.position);
                    attackRangeIndicator.enabled = true;
                    Debug.Log("攻撃したじゅう！");
                    yield return new WaitForSeconds(attackRangeVisualizeDuration); // 攻撃エフェクトの表示時間
                    attackRangeIndicator.enabled = false;
                }
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
