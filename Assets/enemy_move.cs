using UnityEngine;
using System.Collections;

public class Enemy_move : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 2f;
    public int enemyHP = 50;
    public int attackPower = 5;
    public bool canMove = true;

    [SerializeField] public float knockbackPower = 0.3f;
    [SerializeField] public float knockbackDuration = 0.1f;

    void Update()
    {
        if (target == null || !canMove) return;

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    public IEnumerator Knockback(Rigidbody2D rb, Vector2 direction, float power, float duration)
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

    public void DisableColliderAndDestroy()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        Destroy(gameObject);
    }
}
