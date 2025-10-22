using UnityEngine;

public class MikosiController : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    
    static public int mikoshiHP = 100;

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}
