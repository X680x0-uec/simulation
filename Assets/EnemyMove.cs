using UnityEngine;
using System.Collections;

public class EnemyMove : MonoBehaviour
{
    public float moveSpeed = 5f;         // 通常移動速度
    public float knockbackForce = 2f;    // ノックバック力
    public float knockbackTime = 0.3f;   // ノックバック時間
    public int enemyHP = 50;             // HP
    public int attackPower = 5;          // 攻撃力

    private Rigidbody2D rb;
    private bool isKnockedBack = false;  // ノックバック中かどうか

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isKnockedBack)
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
        }
    }

    // ダメージ処理（味方から呼ばれる）
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

        rb.linearVelocity = direction * knockbackForce;

        yield return new WaitForSeconds(knockbackTime);

        isKnockedBack = false;
    }

    public void DisableColliderAndDestroy()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        Destroy(gameObject);
    }
}
