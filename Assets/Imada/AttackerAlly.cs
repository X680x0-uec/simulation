using Unity.VisualScripting;
using UnityEngine;

public class AttackerAlly : MonoBehaviour
{
    [Header("アタッカー味方の設定")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float followDistance = 1.5f; // ミコシを追尾する距離

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
                if (target != null)
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
        rb.linearVelocity = direction * speed;
    }

    void FollowMikoshi()
    {
        // ミコシに向かって移動
        if (mikoshi != null)
        {
            Vector2 distance = mikoshi.position - transform.position;
            if (distance.magnitude > followDistance)
            {
                Vector2 direction = distance.normalized;
                rb.linearVelocity = direction * speed;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {

        }
    }
}
