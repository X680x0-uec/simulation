using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class EnemyBase : MonoBehaviour
// ↓
// 「abstract（抽象クラス）」を付けます。
// これにより、このクラス自体はオブジェクトにアタッチできなくなり、
// 継承（extends）して使うことだけが許可されます。
public abstract class EnemyController : MonoBehaviour
{
    // --- 変数 ---
    // private -> protected に変更
    // protected は「このクラスと、これを継承した子クラスだけ」がアクセスできます。

    [Header("基本設定")]
    [SerializeField] protected int maxHP = 100;
    protected int currentHP;

    [Header("追尾設定")]
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected GameObject mikoshiObject;
    [SerializeField]
    protected List<GameObject> allys_list; // 子クラスがアクセスするため protected に
    [SerializeField] protected float giveUpTime = 2f;
    protected float giveUpTimer;
    protected float distanceToMikoshi;
    protected Vector2 currentPosition;
    //protected GameObject targetPosition; // 未使用のためコメントアウト

    [Header("ノックバック設定")]
    [SerializeField] protected float recoilTime = 0.15f;
    [SerializeField] protected float knockbackPower = 1f;

    [Header("消滅設定")]
    [SerializeField] protected float destroyXPosition = 5f;
    //bool isTarget; // 未使用のためコメントアウト
    protected Rigidbody2D rb;
    protected bool canMove = true;


    // --- 共通の初期化処理 ---
    // protected virtual に変更
    // virtual は「子クラスがこの中身を上書き（override）しても良い」という意味
    protected virtual void Awake()
    {
        // isTarget = true; // 未使用のためコメントアウト
        currentHP = maxHP;
        giveUpTimer = giveUpTime;

        // Allyのリストを作成
        GameObject[] allys = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject ally in allys)
        {
            allys_list.Add(ally);
        }

        // 物理演算コンポーネントを取得
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError(gameObject.name + " に Rigidbody2D が必要じゅう！");

        // ミコシオブジェクトを取得
        if (mikoshiObject == null)
        {
            mikoshiObject = GameObject.FindWithTag("Mikoshi");
        }
    }

    // --- 共通のメインループ ---
    // この Update は final (変更不可) とし、このまま使います。
    // 子クラスは Update を書く代わりに、ExecuteAI() を書きます。
    void Update()
    {
        if (mikoshiObject != null && canMove)
        {
            // AIの「行動」だけを子クラスに任せる
            ExecuteAI();
        }

        // ミコシから一定距離以上離れたら消滅（共通処理）
        if (mikoshiObject.transform.position.x - currentPosition.x > destroyXPosition)
        {
            Destroy(gameObject);
        }
    }

    // --- AIの行動（子クラスが実装する部分） ---
    // 「abstract（抽象メソッド）」
    // 子クラスは、必ずこの ExecuteAI() という名前のメソッドを
    // override して「中身」を実装しなければならない、というルールを課します。
    protected abstract void ExecuteAI();


    // --- 共通の機能：ターゲット追尾 ---
    // 子クラスから呼び出せるよう protected に変更
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

    // --- 共通の機能：ミコシとの衝突 ---
    // この処理は全敵共通なので、private のままでOK
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Mikoshi"))
        {
            // --- ① ダメージを与える ---
            // MikoshiController.mikoshiHP -= 10;
            SoundManager.instance.PlayMikoshiAttacked();//SE

            // --- ② ノックバック ---
            StartCoroutine(Knockback());
        }
    }

    // --- 共通の機能：ノックバック処理（ミコシ衝突時） ---
    // 子クラスが上書きできるよう protected virtual に変更
    protected virtual IEnumerator Knockback()
    {
        canMove = false;
        Vector3 backDir = (transform.position - mikoshiObject.transform.position).normalized;
        rb.AddForce(backDir * knockbackPower, ForceMode2D.Impulse);
        yield return new WaitForSeconds(recoilTime);
        canMove = true;
    }

    // --- 共通の機能：被ダメージ処理 ---
    // public（外部から呼ばれるため）
    public void TakeDamage(int damage, Vector3 attackerPosition)
    {
        currentHP -= damage;
        SoundManager.instance.PlayEnemyAttacked();//SE
        Debug.Log(gameObject.name + "は" + damage + "のダメージを受けたじゅう！ 残りHP: " + currentHP);
        if (currentHP <= 0)
        {
            Debug.Log(gameObject.name + "は倒れたじゅう！");
            SoundManager.instance.PlayEnemyDefeated();//SE
            Destroy(gameObject);
            return;
        }

        StartCoroutine(KnockbackFromAttack(attackerPosition));
    }

    // --- 共通の機能：ノックバック処理（被ダメージ時） ---
    // 子クラスが上書きできるよう protected virtual に変更
    protected virtual IEnumerator KnockbackFromAttack(Vector3 attackerPosition)
    {
        canMove = false;
        Vector3 backDir = (transform.position - attackerPosition).normalized;
        rb.AddForce(backDir * knockbackPower, ForceMode2D.Impulse);
        yield return new WaitForSeconds(recoilTime);
        canMove = true;
    }

    // --- 共通の機能：ターゲット再検索 ---
    // public（外部から呼べるように）
    public void changetarget()
    {
        allys_list.Clear();
        GameObject[] allys = GameObject.FindGameObjectsWithTag("Ally");
        foreach (GameObject ally in allys)
        {
            allys_list.Add(ally);
        }
    }
}