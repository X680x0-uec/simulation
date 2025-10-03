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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mikoshi = GameObject.FindWithTag("Mikoshi").transform;
    }

    void Update()
    {
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
