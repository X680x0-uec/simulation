using UnityEngine;
using System.Collections;

public class EnemyMove : MonoBehaviour
{
    [Header("基本ステータス")]
    public float moveSpeed = 5f;         // 通常移動速度
    public int enemyHP = 50;             // HP
    public int attackPower = 5;          // 攻撃力

    [Header("ノックバック設定")]
    public float knockbackForce = 2f;    // ノックバック力
    public float knockbackTime = 0.3f;   // ノックバック時間

    private Rigidbody2D rb;
    private bool isKnockedBack = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError("EnemyMove に Rigidbody2D が必要です");
    }

    void FixedUpdate()
    {
        // ノックバック中でなければ通常移動
        if (!isKnockedBack)
        {
            rb.AddForce(Vector2.left * moveSpeed, ForceMode2D.Force);
        }
    }

    // 味方から呼ばれるダメージ処理
    public void TakeDamage(int damage, Vector3 attackerPosition)
    {
        enemyHP -= damage;
        if (enemyHP <= 0)
        {
            DisableColliderAndDestroy();
            return;
        }

        Vector2 knockDir = (transform.position - attackerPosition).normalized;
        StartCoroutine(Knockback(knockDir));
    }

    private IEnumerator Knockback(Vector2 direction)
    {
        isKnockedBack = true;

        // 瞬間ノックバック
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);

        // ノックバック時間待機
        yield return new WaitForSeconds(knockbackTime);

        isKnockedBack = false;
    }

    private void DisableColliderAndDestroy()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        Destroy(gameObject);
    }
}
