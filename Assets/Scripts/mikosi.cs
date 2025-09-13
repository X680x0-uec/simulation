using UnityEngine;

public class mikosi : MonoBehaviour
{
    [SerializeField]
    public float speed = 2f;

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }

}
