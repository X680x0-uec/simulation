using UnityEngine;
using System.Collections;

public class AllyMove : MonoBehaviour
{
    [Header("基本ステータス")]
    public float allySpeed = 5f;         // 移動スピード
    public int allyHP = 100;             // HP
    public int attackPower = 10;         // 攻撃力

    [Header("戦闘設定")]
    public float attackCooldown = 0.5f;  // 攻撃間隔
    public float knockbackPower = 2f;    // ノックバック力
    public float knockbackDuration = 0.1f; // ノックバック時間

    private bool isFighting = false;
    private bool canAttack = true;
    private EnemyMove enemyTarget;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError("AllyMove に Rigidbody2D が必要じゅう！");
    }

    void FixedUpdate()
    {
        // 戦闘中でなければ前進
        if (!isFighting)
        {
            rb.AddForce(Vector2.right * allySpeed, ForceMode2D.Force);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("enemy"))
        {
            EnemyMove enemy = other.GetComponent<EnemyMove>();
            if (enemy != null && !isFighting)
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

    private void Update()
    {
        // 攻撃処理
        if (isFighting && enemyTarget != null && canAttack)
        {
            StartCoroutine(Attack(enemyTarget));
        }

        // HPチェック
        if (allyHP <= 0)
        {
            DisableColliderAndDestroy();
        }
    }

    private IEnumerator Attack(EnemyMove enemy)
    {
        canAttack = false;

        // 敵にダメージ + ノックバック
        enemy.TakeDamage(attackPower, transform.position);

        // 味方も敵の攻撃力分ダメージ
        allyHP -= enemy.attackPower;

        // 自身も短くノックバック
        rb.AddForce(Vector2.left * knockbackPower, ForceMode2D.Impulse);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;

        if (allyHP <= 0)
        {
            DisableColliderAndDestroy();
        }
    }

    private void DisableColliderAndDestroy()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        Destroy(gameObject);
    }
}
