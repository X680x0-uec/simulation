using UnityEngine;

public class MikoshiController : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    
    static public int mikoshiHP = 100;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rb.linearVelocity = Vector2.right * speed;
    }
}
