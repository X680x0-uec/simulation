using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;

public class AttackerAlly : MonoBehaviour
{
    [Header("アタッカー味方の設定")]
    [SerializeField] private int HP = 100;
    [SerializeField] private int damage = 10;
    [SerializeField] private float speed = 2f;

    private enum State
    {
        Idle,
        Attack,
        Dead
    }
    private State state = State.Idle;

    private Transform target;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                // 待機状態の処理
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {

        }
    }
}
