using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    // このBGMManagerが存在するかどうかをチェックするための変数
    public static BGMManager instance = null;

    void Awake()
    {
        // まだBGMManagerが（シーン内に）存在しない場合
        if (instance == null)
        {
            // このBGMManagerをinstanceとして登録する
            instance = this;
            // シーンを切り替えても破棄されないようにする
            DontDestroyOnLoad(this.gameObject);
        }
        // すでにBGMManagerが存在する場合（例：Titleに戻ってきた時）
        else
        {
            // このオブジェクトは不要なので破棄する
            Destroy(this.gameObject);
        }
    }
}