using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    private GameObject mikoshiObject;
    private Vector2 currentPosition;
    private Vector2 targetPosition;
    
    void Start()
    {
        mikoshiObject  = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        currentPosition = transform.position;
        targetPosition = mikoshiObject.GetComponent<Transform>().position;
        Vector2 direction = (targetPosition - currentPosition).normalized;
        currentPosition += direction * speed * Time.deltaTime;

        transform.position = currentPosition;

        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
