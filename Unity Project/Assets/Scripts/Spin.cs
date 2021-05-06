using UnityEngine;

public class Spin : MonoBehaviour
{
    public float turnSpeed = 10;

    void Update()
    {
        transform.Rotate(transform.forward * turnSpeed * Time.deltaTime);
    }
}
