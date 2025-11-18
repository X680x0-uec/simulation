using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RangedEnemyController : EnemyController
{
    [SerializeField] private float range = 1f; // 射程
    [SerializeField] private float firstRange = 0.5f; // 感知距離

    private bool attackMode = false;

    [Header("攻撃設定")]
    private float spawnTimer = 0f;
        
    [SerializeField] private float attackInterval = 1f; //攻撃間隔

    [SerializeField] private GameObject enemyPrefab; //弾丸
    [SerializeField] private EnemyManager enemyManager;

    protected override void Awake()
    {
        base.Awake();

        if (enemyManager == null)
        {
            enemyManager = FindFirstObjectByType<EnemyManager>();
        }

        // オブジェクトの色を青に変更(追尾モード)
        GetComponent<Renderer>().material.color = Color.blue;

        distanceToMikoshi = 9999f;
    }

    // FollowTarget がオーバーライドできなかったので、新たに FollowTargetSP を作成
    protected void FollowTargetSP(GameObject target)
    {
        currentPosition = transform.position;
        Vector2 direction = ((Vector2)target.transform.position - currentPosition).normalized;

        if (!attackMode) 
        {
            rb.AddForce(direction * speed);
        }

        if (distanceToMikoshi <= firstRange && !attackMode)
        {
            attackMode = true;
            // オブジェクトの色を赤に変更(攻撃モード)
            GetComponent<Renderer>().material.color = Color.red;
        } else if (distanceToMikoshi >= range && attackMode)
        {
            attackMode = false;
            // オブジェクトの色を青に変更(追尾モード)
            GetComponent<Renderer>().material.color = Color.blue;
        }

        distanceToMikoshi = Vector2.Distance(currentPosition, target.transform.position);
    }

    protected override void ExecuteAI()
    {
        // 移動
        FollowTargetSP(mikoshiObject);

        // 攻撃
        if (attackMode == true)
        {
            if (Time.time - spawnTimer >= attackInterval)
            {
                enemyManager.SpawnBullet(enemyPrefab, currentPosition, gameObject, mikoshiObject.transform.position);
                spawnTimer = Time.time;
            }
        }
    }
}