using UnityEngine;

public class AllyArrowMove : MonoBehaviour
{
	private Vector2 moveDirection;
	private int attackPower;
	private Rigidbody2D rb;

	public void Init(Vector2 direction, int power)
	{
		moveDirection = direction;
		attackPower = power;
		rb = GetComponent<Rigidbody2D>();
		if (rb != null)
		{
			rb.linearVelocity = moveDirection * 10f; // 矢の速度（必要に応じて調整）
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Enemy"))
		{
			EnemyController enemy = other.GetComponent<EnemyController>();
			if (enemy != null)
			{
				enemy.TakeDamage(attackPower, transform.position);
			}
			Destroy(gameObject); // 矢を消す
		}
	}
}
