using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class EnemyMikoshiChaser : MonoBehaviour
// etc...
// EnemyBase used here
public class EnemyMikoshiChaser : EnemyController
{
    // （基底クラス EnemyBase）で必要な処理を実装している
    // ExecuteAI() の挙動は「追跡（override）」を行います。
    protected override void ExecuteAI()
    {
        // この敵のAIは「神輿を追う」ものです。
        // FollowTarget は対象を追尾する基本的な関数です。
        FollowTarget(mikoshiObject);
    }
}