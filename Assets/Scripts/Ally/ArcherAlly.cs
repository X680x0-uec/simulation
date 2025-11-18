using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ArcherAlly : MonoBehaviour
{
    [Header("矢プレハブと攻撃間隔")]
    public GameObject allyArrowPrefab;
    [SerializeField] private float attackInterval = 1.0f;
    private float attackTimer = 0f;
    [Header("アタッカー味方の設定")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private int attackPower = 10;
    [SerializeField] private float sightRange = 8f;

    [Header("追従設定")]
    [SerializeField] private bool isFrontDefense = false;
    [SerializeField] private float frontDefenseAngle = 90f;
    [SerializeField] private float moveThreshold = 1.0f;
    [SerializeField] private float followDistance = 2.0f;
    [SerializeField] private float moveIntervalTime = 1.0f;

    [Header("攻撃時の距離設定")]
    [SerializeField] private float attackKeepDistance = 2.0f;

    private float moveTimer = 0f;
    private Vector2 destination;

    // リロード関連
    [Header("リロード設定")]
    [SerializeField] private int shotsBeforeReload = 3;
    [SerializeField] private float reloadDuration = 2.0f;
    [SerializeField] private float reloadSpeedMultiplier = 0.5f;
    [SerializeField] private float reloadDistanceSlack = 0.5f;
    private int shotCount = 0;
    private bool isReloading = false;
    private float reloadTimer = 0f;

    private enum State
    {
        Idle,
        Attack,
        Dead
    }

    // ★追加: アニメーターとレンダラーの変数
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Transform GetNearestEnemyToPlayer()
    {
        Transform reference = playerTransform != null ? playerTransform : transform;
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies == null || enemies.Length == 0) return null;

        Transform best = null;
        float bestDist = float.MaxValue;
        foreach (var e in enemies)
        {
            float d = Vector2.Distance(reference.position, e.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = e.transform;
            }
        }
        return best;
    }
    private State state = State.Idle;

    private Transform mikoshi;
    private Transform playerTransform;
    private Transform target;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // ★追加: コンポーネントの取得
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        mikoshi = GameObject.FindWithTag("Mikoshi").transform;
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            var byName = GameObject.Find("Player");
            if (byName != null) playerTransform = byName.transform;
        }
    }

    void Update()
    {
        // ★追加: 向きの制御（常に移動方向や敵の方を向く）
        UpdateFacingDirection();

        switch (state)
        {
            case State.Idle:
                // ★追加: アニメーションを「歩き（isAttacking = false）」にする
                if (animator != null) animator.SetBool("isAttacking", false);

                FollowMikoshi();

                // 自機に近い敵をターゲットにする
                target = GetNearestEnemyToPlayer();
                if (target != null && Vector2.Distance(transform.position, target.position) <= sightRange)
                {
                    state = State.Attack;
                    attackTimer = 0f;
                }
                break;

            case State.Attack:
                // ★追加: アニメーションを「攻撃（isAttacking = true）」にする
                if (animator != null) animator.SetBool("isAttacking", true);

                if (target != null)
                {
                    float distanceToTarget = Vector2.Distance(transform.position, target.position);
                    if (distanceToTarget > sightRange)
                    {
                        target = null;
                        state = State.Idle;
                    }
                    else
                    {
                        MoveToTarget();
                        if (!isReloading)
                        {
                            attackTimer -= Time.deltaTime;
                            if (attackTimer <= 0f)
                            {
                                ShootArrow();
                                attackTimer = attackInterval;
                            }
                        }
                        else
                        {
                            reloadTimer -= Time.deltaTime;
                            if (reloadTimer <= 0f)
                            {
                                isReloading = false;
                                shotCount = 0;
                            }
                        }
                    }
                }
                else
                {
                    state = State.Idle;
                }
                break;

            case State.Dead:
                break;
        }
    }

    // ★追加: キャラクターの向き（左右反転）を制御するメソッド
    void UpdateFacingDirection()
    {
        if (target != null)
        {
            // ターゲットがいる時はターゲットの方を向く
            if (target.position.x < transform.position.x)
                spriteRenderer.flipX = true; // 左にいるなら反転（画像が右向きの場合）
            else
                spriteRenderer.flipX = false;
        }
        else if (rb.linearVelocity.magnitude > 0.1f) // 移動中なら
        {
            // 移動方向を向く
            if (rb.linearVelocity.x < -0.1f)
                spriteRenderer.flipX = true; // 左移動
            else if (rb.linearVelocity.x > 0.1f)
                spriteRenderer.flipX = false; // 右移動
        }
        // ※補足: あなたの用意した絵が「左向き」の場合は、true/falseを逆にしてください
    }

    void MoveToTarget()
    {
        float distance = Vector2.Distance(transform.position, target.position);
        Vector2 direction = (target.position - transform.position).normalized;
        float currentSpeed = speed;
        float keepDistance = attackKeepDistance;
        if (isReloading)
        {
            currentSpeed *= reloadSpeedMultiplier;
            keepDistance += reloadDistanceSlack;
        }

        if (distance > keepDistance)
        {
            rb.AddForce(direction * currentSpeed);
        }
        else if (distance < keepDistance * 0.8f)
        {
            rb.AddForce(-direction * currentSpeed);
        }
    }

    void FollowMikoshi()
    {
        if (mikoshi != null && moveTimer <= 0f)
        {
            float randomAngle;
            if (isFrontDefense)
            {
                randomAngle = Random.Range(-frontDefenseAngle, frontDefenseAngle) * Mathf.Deg2Rad;
            }
            else
            {
                randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            }
            moveTimer = moveIntervalTime;
            destination = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * followDistance + (Vector2)mikoshi.position;
        }
        else
        {
            Vector2 direction = destination - (Vector2)transform.position;
            if (direction.magnitude > moveThreshold)
            {
                rb.AddForce(direction.normalized * speed);
            }
            else
            {
                destination += Utils.GetVelocityOfGameObject2D(mikoshi.gameObject) * Time.deltaTime;
                rb.linearVelocity = Utils.GetVelocityOfGameObject2D(mikoshi.gameObject);
            }
            moveTimer -= Time.deltaTime;
        }
    }

    void ShootArrow()
    {
        if (allyArrowPrefab != null && target != null)
        {
            Vector2 spawnPos = transform.position;
            Vector2 direction = (target.position - transform.position).normalized;
            GameObject arrow = Instantiate(allyArrowPrefab, spawnPos, Quaternion.identity);

            var arrowScript = arrow.GetComponent<AllyArrowMove>();
            if (arrowScript != null)
            {
                arrowScript.Init(direction, attackPower);
            }
            // ★追加: 攻撃音をSoundManagerで鳴らす（実装済みなら）
            if (SoundManager.instance != null)
            {
                // 弓の発射音などがあればここで鳴らす
                // SoundManager.instance.PlayArrowShot(transform.position);
            }

            shotCount++;
            if (shotCount >= shotsBeforeReload)
            {
                isReloading = true;
                reloadTimer = reloadDuration;
            }
        }
    }
}