using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : EnemyController
{
    protected override IEnumerator Knockback()
    {
        Destroy(gameObject);
        yield break;
    }
    [SerializeField]
    private float homingDuration = 1f; // 生成後何秒間ホーミングするか
    private float homingTimer;

    protected override void Awake()
    {
        base.Awake();
        homingTimer = homingDuration;
    }

    protected override void ExecuteAI()
    {
        // 最初の homingDuration 秒だけミコシを追尾する
        if (homingTimer > 0f && mikoshiObject != null)
        {
            FollowTarget(mikoshiObject);
            homingTimer -= Time.deltaTime;
        }
        // homing が終わったら追尾は行わない（慣性で直進する想定）
    }
}