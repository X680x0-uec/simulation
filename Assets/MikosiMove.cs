using UnityEngine;
using UnityEngine.UI;

public class MikosiMove : MonoBehaviour
{
    [SerializeField] public float speed = 2f;
    public static int mikosiHP = 100;

    [Header("UI表示用")]
    public Text mikosiHPText;  // Inspectorでセット

    void Update()
    {
        // 移動
        transform.position += Vector3.right * speed * Time.deltaTime;

        // HP表示更新
        if (mikosiHPText != null)
            mikosiHPText.text = "Mikosi HP: " + mikosiHP;
    }
}
