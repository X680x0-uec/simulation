using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class EnemyMikoshiChaser : MonoBehaviour
// ↓
// EnemyBase を継承します
public class EnemyMikoshiChaser : EnemyController
{
    // 親（EnemyBase）が「必ず書け」と命令した
    // ExecuteAI() の中身を「上書き（override）」します。
    protected override void ExecuteAI()
    {
        // この敵のAIは「ミコシを追う」だけ。
        // FollowTarget は親が持っている共通機能を使います。
        FollowTarget(mikoshiObject);
    }
}