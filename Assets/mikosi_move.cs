using UnityEngine;
using System.Collections;

public class mikosi_move : MonoBehaviour  // クラス名とファイル名を一致
{
    [SerializeField]
    public float speed = 2f;       // 自機の速度
    public static int mikosiHP = 100;     // 自機HP

    void Update()
    {
        // 常に右方向に進む
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}
