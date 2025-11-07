using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class EnemyAllyChaser : MonoBehaviour
// ↓
// EnemyBase を継承します
public class EnemyAllyChaser : EnemyController
{
    // このAIだけで使う変数
    private int targetnumber;

    // 親（EnemyBase）が「必ず書け」と命令した
    // ExecuteAI() の中身を「上書き（override）」します。
    protected override void ExecuteAI()
    {
        // この敵のAIは「Allyをランダムに追う」
        if (allys_list.Count != 0)
        {
            // 毎フレーム、ターゲットをランダムに選び直す
            targetnumber = Random.Range(0, allys_list.Count);

            // 共通機能の FollowTarget を使って追尾
            FollowTarget(allys_list[targetnumber]);
        }
        else
        {
            // もしAllyが全滅していたら、代わりにミコシを追う（保険）
            FollowTarget(mikoshiObject);
        }
    }
}