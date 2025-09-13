using UnityEngine;
using System;

public class LineMover : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float speed = 1f;

    private int currentIndex;
    public EventHandler OnEndReached;

    private void Start()
    {
        Initialize(0,speed,lineRenderer);
    }
    public void Initialize(int index, float speed, LineRenderer lineRenderer)
    {
        enabled = true;
        currentIndex = index;
        this.speed = speed;
        this.lineRenderer = lineRenderer;
        transform.position = this.lineRenderer.GetPosition(currentIndex);
    }

    public static (Vector3 targetPosition, bool isEnd) GetTargetPosition(ref int index, float moveSpeed, Vector3 currentPosition, LineRenderer lineRenderer)
    {
        int nextIndex = index + 1;

        if (lineRenderer.positionCount <= nextIndex)
        {
            return (lineRenderer.GetPosition(index)+ lineRenderer.transform.position, true);
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
    private void Update()
    {
        var result = GetTargetPosition(
            ref currentIndex,
            speed * Time.deltaTime,
            transform.position,
            lineRenderer);
        transform.position = result.targetPosition;
        if (result.isEnd)
        {
            OnEndReached?.Invoke(this, EventArgs.Empty);
            enabled = false;
        }
    }
}