using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    private Transform target;

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("UserCamera").transform;
    }

    void Update()
    {
        transform.LookAt(target);
    }
}
