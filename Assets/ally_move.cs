using UnityEngine;
using System.Collections;

public class ally_move : MonoBehaviour
{
    [SerializeField] public float allySpeed = 2f;       // 移動速度
    [SerializeField] public int allyHP = 100;           // HP
    [SerializeField] public int attackPower = 10;       // 攻撃力
    [SerializeField] public float attackInterval = 0.5f; // 攻撃間隔
    [SerializeField] public float knockbackPower = 0.3f; // ノックバック距離
    [SerializeField] public float knockbackDuration = 0.1f; // ノックバック時間

    private bool isFighting = false;
    private Enemy_move enemyTarget;

    void Update()
    {
        if (!isFighting)
        {
            transform.position += Vector3.right * allySpeed * Time.deltaTime;
        }

        if (allyHP <= 0)
        {
            DisableColliderAndDestroy();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("enemy"))
        {
            Enemy_move enemy = other.GetComponent<Enemy_move>();
            if (enemy != null && !isFighting)
            {
                StartCoroutine(Fight(enemy));
            }
        }
    }

    IEnumerator Fight(Enemy_move enemy)
    {
        isFighting = true;

        Rigidbody2D allyRb = GetComponent<Rigidbody2D>();
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();

        // 敵の移動停止
        enemy.canMove = false;

        while (enemy != null && enemy.enemyHP > 0 && allyHP > 0)
        {
            // ダメージ
            enemy.enemyHP -= attackPower;
            allyHP -= enemy.attackPower;

            // 互いにノックバック
            yield return StartCoroutine(Knockback(allyRb, Vector2.left, knockbackPower, knockbackDuration));
            yield return StartCoroutine(Knockback(enemyRb, Vector2.right, knockbackPower, knockbackDuration));

            yield return new WaitForSeconds(attackInterval);
        }

        // 敵死亡処理
        if (enemy != null && enemy.enemyHP <= 0)
        {
            enemy.DisableColliderAndDestroy();
        }
        else
        {
            enemy.canMove = true; // 生き残ったら移動再開
        }

        if (allyHP <= 0)
        {
            DisableColliderAndDestroy();
        }

        isFighting = false;
    }


    IEnumerator Knockback(Rigidbody2D rb, Vector2 direction, float power, float duration)
    {
        if (rb == null) yield break;

        Vector2 startPos = rb.position;
        Vector2 endPos = startPos + direction * power;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rb.MovePosition(Vector2.Lerp(startPos, endPos, elapsed / duration));
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    void DisableColliderAndDestroy()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        Destroy(gameObject);
    }
}