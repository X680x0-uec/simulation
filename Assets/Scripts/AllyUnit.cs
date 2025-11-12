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
    public AllyManager OwnerManager { get; set; }

    private void Awake()
    {
        CurrentHP = maxHP;
    }

    /// <summary>
    /// Initialize sets max hp and type index. Call this after instantiating a prefab.
    /// </summary>
    public void Initialize(int hp, int type)
    {
        maxHP = hp;
        CurrentHP = maxHP;
        typeIndex = type;
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            Die();
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
