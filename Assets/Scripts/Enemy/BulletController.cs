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
    protected override void ExecuteAI()
    {
        FollowTarget(mikoshiObject);
    }
}