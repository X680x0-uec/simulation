using UnityEngine;
using System.Collections;

public class AllyMove : MonoBehaviour
{
    [Header("基本ステータス")]
    [SerializeField] public float allySpeed = 2f;        // 移動速度
    [SerializeField] public int allyHP = 100;            // HP
    [SerializeField] public int attackPower = 10;        // 攻撃力

    [Header("戦闘設定")]
    [SerializeField] public float attackCooldown = 0.5f; // 攻撃間隔
    [SerializeField] public float knockbackPower = 0.3f; // ノックバック距離
    [SerializeField] public float knockbackDuration = 0.1f; // ノックバック時間

    private bool isFighting = false;
    private bool canAttack = true;
    private EnemyMove enemyTarget;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 戦闘中でなければ前進
        if (!isFighting)
        {
            transform.position += Vector3.right * allySpeed * Time.deltaTime;
        }

        // HP0で死亡処理
        if (allyHP <= 0)
        {
            DisableColliderAndDestroy();
        }

        // 敵がいて攻撃可能なら攻撃
        if (isFighting && enemyTarget != null && canAttack)
        {
            StartCoroutine(Attack(enemyTarget));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("enemy"))
        {
            EnemyMove enemy = other.GetComponent<EnemyMove>();
            if (enemy != null)
            {
                enemyTarget = enemy;
                isFighting = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("enemy") && enemyTarget != null && other.gameObject == enemyTarget.gameObject)
        {
            enemyTarget = null;
            isFighting = false;
        }
    }

    private IEnumerator Attack(EnemyMove enemy)
    {
        canAttack = false;

        // ダメージ与える
        enemy.TakeDamage(attackPower, transform.position);

        // 味方もダメージを受ける
        allyHP -= enemy.attackPower;

        // ノックバック演出
        yield return StartCoroutine(Knockback(rb, Vector2.left, knockbackPower, knockbackDuration));

        // クールダウン待機
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;

        // 死亡判定
        if (allyHP <= 0)
        {
            DisableColliderAndDestroy();
        }
    }

    private IEnumerator Knockback(Rigidbody2D rb, Vector2 direction, float power, float duration)
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

    private void DisableColliderAndDestroy()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        Destroy(gameObject);
    }
}
