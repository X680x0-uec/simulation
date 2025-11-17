using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class RangedEnemyController : MonoBehaviour
{
    [Header("基本設定")]
    [SerializeField] private int maxHP = 100;
    private int currentHP;

    [Header("追尾設定")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private GameObject mikoshiObject;
    [SerializeField] private float giveUpTime = 2f; // ミコシに近づけなくなったら諦める時間
    [SerializeField] private float range = 1f; // 射程
    [SerializeField] private float firstRange = 0.5f; // 感知距離
    private float giveUpTimer;
    private float distanceToMikoshi = 9999f;

    private Vector2 currentPosition;
    private Vector2 targetPosition;

    [Header("ノックバック設定")]
    [SerializeField] private float recoilTime = 0.15f; // 硬直時間
    [SerializeField] private float knockbackPower = 1f;         // ノックバックの強さ

    [Header("消滅設定")]
    [SerializeField] private float destroyXPosition = 5f; // ミコシからのX距離がこれ以上離れたら消滅

    [Header("攻撃設定")]
    [SerializeField] private GameObject enemyPrefab; //弾丸
    [SerializeField] private float attackInterval = 1f; //攻撃間隔
    private float spawnTimer = 0f;
 
    private Rigidbody2D rb;
    private bool canMove = true;
    private bool attackMode = false;

    void Awake()
    {
        currentHP = maxHP;
        giveUpTimer = giveUpTime;
        
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError("EnemyController に Rigidbody2D が必要じゅう！");
        if (mikoshiObject == null)
        {
            mikoshiObject = GameObject.FindWithTag("Mikoshi");
        }

        // オブジェクトの色を青に変更(追尾モード)
        GetComponent<Renderer>().material.color = Color.blue;
    }

    void Update()
    {
        if (mikoshiObject != null)
        {
            FollowTarget();
        }

        // ミコシから一定距離以上離れたら消滅
        if (targetPosition.x - currentPosition.x > destroyXPosition)
        {
            Destroy(gameObject);
        }

        if (attackMode == true)
        {
            if (Time.time - spawnTimer >= attackInterval)
            {
                SpawnEnemy();
                spawnTimer = Time.time;
            }
        }
    }

    void FollowTarget()
    {
        currentPosition = transform.position;
        targetPosition = mikoshiObject.GetComponent<Transform>().position;
        Vector2 direction = (targetPosition - currentPosition).normalized;

        if (canMove) 
        {
            rb.AddForce(direction * speed);
        }

        if (distanceToMikoshi <= firstRange && !attackMode)
        {
            canMove = false;
            attackMode = true;
            // オブジェクトの色を赤に変更(攻撃モード)
            GetComponent<Renderer>().material.color = Color.red;
        } else if (distanceToMikoshi >= range && attackMode)
        {
            canMove = true;
            attackMode = false;
            // オブジェクトの色を青に変更(追尾モード)
            GetComponent<Renderer>().material.color = Color.blue;
            giveUpTimer = giveUpTime;
        }

        // ミコシに近づけなくなったら一定時間後に諦める
        else if (distanceToMikoshi <= Vector2.Distance(currentPosition, targetPosition))
        {
            giveUpTimer -= Time.deltaTime;
            if (giveUpTimer <= 0f)
            {
                // 一定時間後に諦める
                canMove = false;
            }
        }
        else
        {
            giveUpTimer = giveUpTime; // リセット
        }
        distanceToMikoshi = Vector2.Distance(currentPosition, targetPosition);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Mikoshi"))
        {
            // --- ① ダメージを与える ---
            // MikoshiController.mikoshiHP -= 10;

            // --- ② ノックバック ---
            StartCoroutine(Knockback());
        }
    }

    IEnumerator Knockback()
    {
        // 追尾を止める
        canMove = false;

        // 現在の方向と逆方向にノックバック
        Vector3 backDir = (transform.position - mikoshiObject.transform.position).normalized;

        rb.AddForce(backDir * knockbackPower, ForceMode2D.Impulse);

        // 少し待つ
        yield return new WaitForSeconds(recoilTime);

        // 追尾再開
        //canMove = true;
    }

    public void TakeDamage(int damage, Vector3 attackerPosition)
    {
        // --- ① ダメージ処理 ---
        // （ここでは単純にログを出すだけ）
        currentHP -= damage;
        Debug.Log(gameObject.name + "は" + damage + "のダメージを受けたじゅう！ 残りHP: " + currentHP);
        if (currentHP <= 0)
        {
            Debug.Log(gameObject.name + "は倒れたじゅう！");
            Destroy(gameObject);
            return;
        }

        // --- ② ノックバック処理 ---
        StartCoroutine(KnockbackFromAttack(attackerPosition));
    }
    
    IEnumerator KnockbackFromAttack(Vector3 attackerPosition)
    {
        // 追尾を止める
        canMove = false;

        // 攻撃者の方向と逆方向にノックバック
        Vector3 backDir = (transform.position - attackerPosition).normalized;

        rb.AddForce(backDir * knockbackPower, ForceMode2D.Impulse);

        // 少し待つ
        yield return new WaitForSeconds(recoilTime);

        // 追尾再開
        canMove = true;
    }

    void SpawnEnemy()
    {
        Instantiate(enemyPrefab, currentPosition, Quaternion.identity);
    }
}