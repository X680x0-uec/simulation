using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // 追う対象（神輿）
    [SerializeField] private Vector3 offset = new Vector3(-3f, 0f, -10f);
    [SerializeField] private float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        // 追従位置を計算
        Vector3 desiredPosition = target.position + offset;

        // スムーズに移動
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
