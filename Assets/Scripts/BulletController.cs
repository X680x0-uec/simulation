using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class BulletController : MonoBehaviour
{
    [Header("基本設定")]
    [SerializeField] private int maxHP = 100;
    private int currentHP;

    [Header("追尾設定")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private GameObject mikoshiObject;
    [SerializeField] private float giveUpTime = 2f; // ミコシに近づけなくなったら諦める時間
    private float giveUpTimer;
    private float distanceToMikoshi;

    private Vector2 currentPosition;
    private Vector2 targetPosition;

    [Header("消滅設定")]
    [SerializeField] private float destroyXPosition = 5f; // ミコシからのX距離がこれ以上離れたら消滅

    private Rigidbody2D rb;
    private bool canMove = true;

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
    }

    void Update()
    {
        if (mikoshiObject != null && canMove)
        {
            FollowTarget();
        }

        // ミコシから一定距離以上離れたら消滅
        if (targetPosition.x - currentPosition.x > destroyXPosition)
        {
            Destroy(gameObject);
        }
    }

    void FollowTarget()
    {
        currentPosition = transform.position;
        targetPosition = mikoshiObject.GetComponent<Transform>().position;
        Vector2 direction = (targetPosition - currentPosition).normalized;

        rb.AddForce(direction * speed);

        // ミコシに近づけなくなったら一定時間後に諦める
        if (distanceToMikoshi <= Vector2.Distance(currentPosition, targetPosition))
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

            Destroy(gameObject);
        }
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
    }
    

}