using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DefenseAlly : MonoBehaviour
{

    [Header("半円配置")]
    public Transform mikoshi;       // 自機（スクリプト参照）
    public int index;
    public int totalAllies;
    public float radius = 3f;
    public float moveSpeed = 5f;

    [Header("攻撃設定")]
    public int attackPower = 10;
    public float attackCooldown = 0.5f;

    private List<Collider2D> enemiesInRange = new List<Collider2D>();
    private bool canAttack = true;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError("DefenseAlly に Rigidbody2D が必要じゅう！");
    }

    void Start()
    {
        if (mikoshi == null)
        {
            GameObject obj = GameObject.FindWithTag("Mikoshi");
            if (obj != null) mikoshi = obj.transform;
            else Debug.LogError("タグ: Mikoshi が見つからないじゅう！");
        }
    }

    void Update()
    {
        if (mikoshi == null) return;

        // 半円追従
        float angleStep = 180f / (totalAllies + 1);
        float angle = -90f + angleStep * (index + 1);
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
        Vector3 targetPos = mikoshi.position + offset;

        rb.MovePosition(Vector2.Lerp(rb.position, (Vector2)targetPos, Time.deltaTime * moveSpeed));

        // 攻撃
        if (canAttack && enemiesInRange.Count > 0)
        {
            StartCoroutine(AttackAllEnemies());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("enemy") && !enemiesInRange.Contains(other))
        {
            enemiesInRange.Add(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("enemy") && enemiesInRange.Contains(other))
        {
            enemiesInRange.Remove(other);
        }
    }

    private IEnumerator AttackAllEnemies()
    {
        canAttack = false;

        foreach (Collider2D enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                EnemyMove enemyScript = enemy.GetComponent<EnemyMove>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(attackPower, transform.position);
                }
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
