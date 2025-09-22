using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class EnemyController : MonoBehaviour
{
    [Header("追尾設定")]
    [SerializeField] private float speed = 2f;
    private GameObject mikoshiObject;
    private Vector2 currentPosition;
    private Vector2 targetPosition;

    [Header("ノックバック設定")]
    [SerializeField] private float recoilTime = 0.15f; // 硬直時間
    public float knockbackPower = 2f;         // ノックバックの強さ

    [Header("消滅設定")]
    [SerializeField] private float destroyXPosition = 5f; // ミコシからのX距離がこれ以上離れたら消滅

    private bool canMove = true;

    void Start()
    {
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
    }

    void FollowTarget()
    {
        currentPosition = transform.position;
        targetPosition = mikoshiObject.GetComponent<Transform>().position;
        Vector2 direction = (targetPosition - currentPosition).normalized;
        currentPosition += direction * speed * Time.deltaTime;

        transform.position = currentPosition;

        if (targetPosition.x - currentPosition.x > destroyXPosition)
        {
            Destroy(gameObject);
        }
    }
 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Mikoshi"))
        {
            // --- ① ダメージを与える ---
            MikoshiController.mikoshiHP -= 10;

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
        transform.position += backDir * knockbackPower;

        // 少し待つ
        yield return new WaitForSeconds(recoilTime);

        // 追尾再開
        canMove = true;
    }
}