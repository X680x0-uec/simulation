using UnityEngine;

/// <summary>
/// Simple HP component for ally units.
/// Initialize via Initialize(maxHp, typeIndex) after instantiation so GameStats and type are known.
/// </summary>
public class AllyUnit : MonoBehaviour
{
    [Header("Default HP (can be overridden via Initialize)")]
    [SerializeField] private int maxHP = 50;

    public int CurrentHP { get; private set; }
    private int typeIndex = -1; // 0=Attacker,1=Defencer,2=Archer
    // Reference back to the manager that spawned this unit (set by AllyManager)
    private float kbPower = 1.0f;
    public AllyManager OwnerManager { get; set; }

    private void Awake()
    {
        CurrentHP = maxHP;
    }

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Enemy")
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                TakeDamage(enemy.attackDamage, collision.transform.position);
            }
        }
	}

	/// <summary>
	/// Initialize sets max hp and type index. Call this after instantiating a prefab.
	/// </summary>
	public void Initialize(int hp, int type, float knockbackPower)
    {
        maxHP = hp;
        CurrentHP = maxHP;
        typeIndex = type;
        kbPower = knockbackPower;
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    // 外部から攻撃座標を渡してノックバック付きでダメージを与えるオーバーロード
    public void TakeDamage(int damage, Vector3 attackerPosition)
    {
        TakeDamage(damage);
        // ノックバック表現（Rigidbody2D があれば軽く弾く）
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 backDir = ((Vector2)(transform.position - attackerPosition)).normalized;
            rb.AddForce(backDir * kbPower, ForceMode2D.Impulse);
        }
    }

    private void Die()
    {
        // notify global stats
        if (typeIndex >= 0)
        {
            GameStats.IncrementAllyDeath(typeIndex);
        }
        // notify manager so it can remove this from its list
        if (OwnerManager != null)
        {
            OwnerManager.OnAllyRemoved(this);
        }
        Destroy(gameObject);
    }
}
