using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    [Header("基本設定")]
    [SerializeField] protected int maxHP = 100;
    protected int currentHP;
    public int attackDamage; // EnemyDatabase から設定される

    [Header("追尾設定")]
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected GameObject mikoshiObject;
    [SerializeField]
    protected List<GameObject> allys_list;
    [SerializeField] protected float giveUpTime = 2f;
    protected float giveUpTimer;
    protected float distanceToMikoshi;
    protected Vector2 currentPosition;

    [Header("ノックバック設定")]
    [SerializeField] protected float recoilTime = 0.15f;
    [SerializeField] protected float knockbackPower = 1f;

    [Header("消滅設定")]
    [SerializeField] protected float destroyXPosition = 5f;
    protected Rigidbody2D rb;
    protected bool canMove = true;

    // EnemyDatabase からパラメータを設定するメソッド
    public void InitializeFromData(int health, int damage)
    {
        maxHP = health;
        currentHP = maxHP;
        attackDamage = damage;
        Debug.Log($"{gameObject.name} initialized: HP={maxHP}, Damage={attackDamage}");
    }

    protected virtual void Awake()
    {
        currentHP = maxHP;
        giveUpTimer = giveUpTime;

        // Allyのリストを作成
        GameObject[] allys = GameObject.FindGameObjectsWithTag("Ally");
        allys_list = new List<GameObject>(allys);

        // 物理演算コンポーネントを取得
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError(gameObject.name + " に Rigidbody2D が必要じゅう！");

        // ミコシオブジェクトを取得
        if (mikoshiObject == null)
        {
            mikoshiObject = GameObject.FindWithTag("Mikoshi");
        }
    }

    void Update()
    {
        if (mikoshiObject != null && canMove)
        {
            ExecuteAI();
        }

        // ミコシから一定距離以上離れたら消滅（共通処理）
        if (mikoshiObject != null && mikoshiObject.transform.position.x - currentPosition.x > destroyXPosition)
        {
            Destroy(gameObject);
        }
    }

    protected abstract void ExecuteAI();

    protected void FollowTarget(GameObject target)
    {
        currentPosition = transform.position;
        Vector2 direction = ((Vector2)target.transform.position - currentPosition).normalized;

        rb.AddForce(direction * speed);

        // ミコシに近づけなくなったら一定時間後に諦める
        if (distanceToMikoshi <= Vector2.Distance(currentPosition, target.transform.position))
        {
            giveUpTimer -= Time.deltaTime;
            if (giveUpTimer <= 0f)
            {
                canMove = false;
            }
        }
        else
        {
            giveUpTimer = giveUpTime; // リセット
        }
        distanceToMikoshi = Vector2.Distance(currentPosition, target.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        if (other.CompareTag("Mikoshi"))
        {
            // MikoshiControllerImada に修正
            MikoshiControllerImada mikoshiScript = other.GetComponent<MikoshiControllerImada>();
            if (mikoshiScript != null)
            {
                mikoshiScript.TakeDamage(attackDamage, transform.position);
                Debug.Log($"{gameObject.name} が神輿に {attackDamage} ダメージを与えた！");
            }

            // SE再生
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlayMikoshiAttacked();
            }

            // ノックバック
            StartCoroutine(Knockback());
        }
    }

    protected virtual IEnumerator Knockback()
    {
        canMove = false;
        if (mikoshiObject != null)
        {
            Vector3 backDir = (transform.position - mikoshiObject.transform.position).normalized;
            rb.AddForce(backDir * knockbackPower, ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(recoilTime);
        canMove = true;
    }

    public void TakeDamage(int damage, Vector3 attackerPosition)
    {
        currentHP -= damage;
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayEnemyAttacked();
        }
        Debug.Log(gameObject.name + "は" + damage + "のダメージを受けたじゅう！ 残りHP: " + currentHP);
        
        if (currentHP <= 0)
        {
            Debug.Log(gameObject.name + "は倒れたじゅう！");
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlayEnemyDefeated();
            }
            Destroy(gameObject);
            return;
        }

        StartCoroutine(KnockbackFromAttack(attackerPosition));
    }

    protected virtual IEnumerator KnockbackFromAttack(Vector3 attackerPosition)
    {
        canMove = false;
        Vector3 backDir = (transform.position - attackerPosition).normalized;
        rb.AddForce(backDir * knockbackPower, ForceMode2D.Impulse);
        yield return new WaitForSeconds(recoilTime);
        canMove = true;
    }

    public void changetarget()
    {
        allys_list.Clear();
        GameObject[] allys = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject ally in allys)
        {
            allys_list.Add(ally);
        }
    }

    // ダメージ量を取得するメソッド
    public int GetDamage()
    {
        return attackDamage;
    }
}