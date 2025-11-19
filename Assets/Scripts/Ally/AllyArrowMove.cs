using UnityEngine;

public class AllyArrowMove : MonoBehaviour
{
    private Vector2 moveDirection;
    private int attackPower;
    private Rigidbody2D rb;

    public void Init(Vector2 direction, int power)
    {
        moveDirection = direction;
        attackPower = power;
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = moveDirection * 10f; // 矢の速度
        }

        // ★追加: 進行方向に合わせて矢を回転させる処理
        // Atan2を使ってベクトルから角度（ラジアン）を出し、度数（デグリー）に変換
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;

        // 計算した角度をZ軸の回転として適用
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>(); // または EnemyHealth など
            if (enemy != null)
            {
                // 敵にダメージを与える（transform.position はヒットエフェクト用などに使える）
                enemy.TakeDamage(attackPower, transform.position); // ※もしエラー出るなら引数確認
            }

            // ★追加: ヒット音を鳴らすならここ（SoundManager経由など）
            // SoundManager.instance.PlayHitSound(transform.position);

            Destroy(gameObject); // 矢を消す
        }
        // ★追加: 壁などに当たった時も消す処理があると良いかも
        // else if (other.CompareTag("Wall")) { Destroy(gameObject); }
    }

    // ★追加: 画面外に永遠に飛んでいかないように、数秒後に自動消滅させる
    void Start()
    {
        Destroy(gameObject, 5.0f); // 5秒後に消える
    }
}