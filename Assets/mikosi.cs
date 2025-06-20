using UnityEngine;

public class mikosi : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints; // 中継地点をInspectorで指定
    [SerializeField] private float speed = 2f;

    private int currentIndex = 0;

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];
        Vector3 direction = (target.position - transform.position).normalized;
        float step = speed * Time.deltaTime;

        transform.position += direction * step;

        // 到達チェック（近くなったらOKにする）
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentIndex++;

            if (currentIndex >= waypoints.Length)
            {
                currentIndex = 0; // ループする場合（止めたければ処理を変える）
            }
        }
    }
}
