using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AttackerAlly : MonoBehaviour
{
    [Header("アタッカー味方の設定")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private int attackPower = 10;
    [SerializeField] private float sightRange = 8f;

    [Header("追従設定")]
    [SerializeField] private bool isFrontDefense = false; // 前衛かどうか
    [SerializeField] private float frontDefenseAngle = 90f; // 前衛の角度範囲
    [SerializeField] private float moveThreshold = 1.0f;
    [SerializeField] private float followDistance = 2.0f;
    [SerializeField] private float moveIntervalTime = 1.0f;

    private float moveTimer = 0f;
    private Vector2 destination;

    private enum State
    {
        Idle,
        Attack,
        Dead
    }
    private State state = State.Idle;

    private Transform mikoshi;
    private Transform target;
    private Rigidbody2D rb;

    // ★追加: 画像の向きを変えるためのコンポーネント
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // ★追加: 子オブジェクトにあるかもしれないので InChildren で取得
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        mikoshi = GameObject.FindWithTag("Mikoshi").transform;
    }

    void Update()
    {
        // ★追加: 毎フレーム向きを更新
        UpdateFacingDirection();

        switch (state)
        {
            case State.Idle:
                // 待機状態の処理
                FollowMikoshi();

                target = Utils.FetchNearObjectWithTag(transform, "Enemy");
                if (target != null && Vector2.Distance(transform.position, target.position) <= sightRange)
                {
                    state = State.Attack; // 敵を見つけたら攻撃状態に移行
                }
                break;
            case State.Attack:
                // 攻撃状態の処理
                if (target != null)
                {
                    MoveToTarget();
                }
                else
                {
                    state = State.Idle; // 敵がいなくなったら待機状態に戻る
                }
                break;
            case State.Dead:
                // 死亡状態の処理
                break;
        }
    }

    // ★追加: 向き（左右反転）を制御するメソッド
    void UpdateFacingDirection()
    {
        // SpriteRendererがない場合は何もしない（エラー防止）
        if (spriteRenderer == null) return;

        if (target != null)
        {
            // ターゲットがいる時はターゲットの方を向く
            // ターゲットが自分より左(xが小さい)なら反転
            if (target.position.x < transform.position.x)
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;
        }
        else if (Mathf.Abs(rb.linearVelocity.x) > 0.1f) // 移動中なら
        {
            // 移動方向を向く
            if (rb.linearVelocity.x < -0.1f)
                spriteRenderer.flipX = true; // 左移動
            else if (rb.linearVelocity.x > 0.1f)
                spriteRenderer.flipX = false; // 右移動
        }
        // ※もし元の絵が「左向き」の場合は true/false を逆にしてください
    }

    void MoveToTarget()
    {
        // 敵に向かって移動
        Vector2 direction = (target.position - transform.position).normalized;
        rb.AddForce(direction * speed);
    }

    void FollowMikoshi()
    {
        // ミコシに向かって移動
        if (mikoshi != null && moveTimer <= 0f)
        {
            float randomAngle;
            if (isFrontDefense)
            {
                randomAngle = Random.Range(-frontDefenseAngle, frontDefenseAngle) * Mathf.Deg2Rad;
            }
            else
            {
                randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            }
            moveTimer = moveIntervalTime;
            destination = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * followDistance + (Vector2)mikoshi.position;
        }
        else
        {
            Vector2 direction = destination - (Vector2)transform.position;
            if (direction.magnitude > moveThreshold)
            {
                rb.AddForce(direction.normalized * speed);
            }
            else
            {
                destination += Utils.GetVelocityOfGameObject2D(mikoshi.gameObject) * Time.deltaTime;
                rb.linearVelocity = Utils.GetVelocityOfGameObject2D(mikoshi.gameObject);
            }
            moveTimer -= Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 敵に触れたら攻撃
            EnemyController enemyScript = other.GetComponent<EnemyController>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(attackPower, transform.position);
            }
            moveTimer = 0f;
            state = State.Idle; // 攻撃後は待機状態に戻る
        }
    }
}