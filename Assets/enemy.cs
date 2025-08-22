using UnityEngine;
using System.Collections;

public class EnemyFollow : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 5f;
    public float knockbackPower = 2f;         // ノックバックの強さ
    public mikosi_move mikosiScript;              // ← mikosiスクリプトへの参照

    private bool canMove = true;

    void Update()
    {
        if (target == null || !canMove) return;

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("mikosi"))
        {
            // --- ① ダメージを与える ---
            mikosiScript.mikosiHP -= 10;

            // --- ② ノックバック ---
            StartCoroutine(Knockback());
        }
    }

    IEnumerator Knockback()
    {
        // 追尾を止める
        canMove = false;

        // 現在の方向と逆方向にノックバック
        Vector3 backDir = (transform.position - target.position).normalized;
        transform.position += backDir * knockbackPower;

        // 少し待つ（ここを変えると硬直時間が調整できる）
        yield return new WaitForSeconds(0.15f);

        // 追尾再開
        canMove = true;
    }
}
