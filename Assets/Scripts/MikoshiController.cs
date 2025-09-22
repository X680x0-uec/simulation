using UnityEngine;

public class MikoshiController : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private LineRenderer lineRenderer;

    static public int mikoshiHP = 100;

    private int currentIndex;
    private bool isMoving = true;

    // 終点に到達したかどうかのフラグ
    public bool hasReachedEnd { get; private set; } = false;

    void Start()
    {
        BeginMove();
    }

    void Update()
    {
        if (!isMoving) return;

        var result = GetTargetPosition(
            ref currentIndex,
            speed * Time.deltaTime,
            transform.position,
            lineRenderer
        );

        transform.position = result.targetPosition;

        if (result.isEnd)
        {
            isMoving = false;
            hasReachedEnd = true;  // ← 終点に到達したらフラグON
            Debug.Log("神輿がゴールしました");
        }
    }

    public void BeginMove()
    {
        currentIndex = 0;
        isMoving = true;
        hasReachedEnd = false; // ← リセット
        transform.position = lineRenderer.GetPosition(currentIndex) + lineRenderer.transform.position;
    }

    private static (Vector3 targetPosition, bool isEnd) GetTargetPosition(
        ref int index,
        float moveSpeed,
        Vector3 currentPosition,
        LineRenderer lineRenderer)
    {
        int nextIndex = index + 1;

        if (lineRenderer.positionCount <= nextIndex)
        {
            return (lineRenderer.GetPosition(index) + lineRenderer.transform.position, true);
        }

        var nextPosition = lineRenderer.GetPosition(nextIndex) + lineRenderer.transform.position;
        float distance = Vector3.Distance(currentPosition, nextPosition);

        if (distance < moveSpeed)
        {
            index += 1;
            return GetTargetPosition(ref index, moveSpeed - distance, nextPosition, lineRenderer);
        }
        else
        {
            Vector3 direction = (nextPosition - currentPosition).normalized;
            return (currentPosition + direction * moveSpeed, false);
        }
    }
}
