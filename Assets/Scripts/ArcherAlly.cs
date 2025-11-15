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
    [SerializeField] private bool isFrontDefense = false; // 前衛かどうか
    [SerializeField] private float frontDefenseAngle = 90f; // 前衛の角度範囲
    [SerializeField] private float moveThreshold = 1.0f;
    [SerializeField] private float followDistance = 2.0f;
    [SerializeField] private float moveIntervalTime = 1.0f;

    [Header("攻撃時の距離設定")]
    [SerializeField] private float attackKeepDistance = 2.0f; // 敵と保つ距離

    private float moveTimer = 0f;
    private Vector2 destination;

    // リロード（矢を引き絞る）関連
    [Header("リロード設定")]
    [SerializeField] private int shotsBeforeReload = 3;
    [SerializeField] private float reloadDuration = 2.0f; // リロードで鈍る時間
    [SerializeField] private float reloadSpeedMultiplier = 0.5f; // 鈍っている間の速度倍率
    [SerializeField] private float reloadDistanceSlack = 0.5f; // 距離維持の許容幅を広げる量
    private int shotCount = 0;
    private bool isReloading = false;
    private float reloadTimer = 0f;

    private enum State
    {
        Idle,
        Attack,
        Dead
    }

    // Player に最も近い敵を返す（見つからなければ null）
    private Transform GetNearestEnemyToPlayer()
    {
        Transform reference = playerTransform != null ? playerTransform : transform;
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies == null || enemies.Length == 0)
        {
            Debug.Log("GetNearestEnemyToPlayer: No enemies found");
            return null;
        }
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
        Debug.Log($"GetNearestEnemyToPlayer: reference={(reference!=null?reference.name:"null")}, found={(best!=null?best.name:"null")}, dist={bestDist}");
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
        mikoshi = GameObject.FindWithTag("Mikoshi").transform;
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            // Playerタグがない場合は名前 'Player' で探すフォールバック
            var byName = GameObject.Find("Player");
            if (byName != null) playerTransform = byName.transform;
        }
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                // 待機状態の処理
                FollowMikoshi();

                // デバッグ: playerTransform の状態と敵数
                var enemiesNow = GameObject.FindGameObjectsWithTag("Enemy");
                Debug.Log($"ArcherAlly.Idle: playerTransform={(playerTransform!=null?playerTransform.name:"null")}, enemyCount={enemiesNow.Length}");

                // 自機に近い敵をターゲットにする
                target = GetNearestEnemyToPlayer();
                // sightRange 判定は自分との距離で行う（playerTransformが無い場合に対応）
                if (target != null && Vector2.Distance(transform.position, target.position) <= sightRange)
                {
                    Debug.Log("ArcherAlly: Target acquired -> " + target.name);
                    state = State.Attack; // 敵を見つけたら攻撃状態に移行
                    attackTimer = 0f;
                }
                break;
            case State.Attack:
                // 攻撃状態の処理
                if (target != null)
                {
                    float distanceToTarget = Vector2.Distance(transform.position, target.position);
                    if (distanceToTarget > sightRange)
                    {
                        // 敵が視界外に出たら待機状態に戻る
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
                                // リロード中は発射しない。タイマーで復帰処理。
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
                    state = State.Idle; // 敵がいなくなったら待機状態に戻る
                }
                break;
            case State.Dead:
                // 死亡状態の処理
                break;
        }
    }

    void MoveToTarget()
    {
        // 敵と一定距離を保つように移動
        float distance = Vector2.Distance(transform.position, target.position);
        Vector2 direction = (target.position - transform.position).normalized;
        float currentSpeed = speed;
        float keepDistance = attackKeepDistance;
        if (isReloading)
        {
            currentSpeed *= reloadSpeedMultiplier; // 移動が鈍る
            keepDistance += reloadDistanceSlack; // 距離維持の許容を広げる
        }

        if (distance > keepDistance)
        {
            // 距離が離れすぎている場合は近づく
            rb.AddForce(direction * currentSpeed);
        }
        else if (distance < keepDistance * 0.8f)
        {
            // 距離が近すぎる場合は離れる
            rb.AddForce(-direction * currentSpeed);
        }
        // 適切な距離なら何もしない
    }

    void FollowMikoshi()
    {
        // ミコシに向かって移動
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
            // AllyArrowスクリプトに方向や攻撃力を渡す（必要に応じて）
            var arrowScript = arrow.GetComponent<AllyArrowMove>();
            if (arrowScript != null)
            {
                arrowScript.Init(direction, attackPower);
            }
            Debug.Log("ArcherAlly: ShootArrow fired at " + target.name);
            // 射撃カウント管理
            shotCount++;
            if (shotCount >= shotsBeforeReload)
            {
                isReloading = true;
                reloadTimer = reloadDuration;
            }
        }
    }
}
