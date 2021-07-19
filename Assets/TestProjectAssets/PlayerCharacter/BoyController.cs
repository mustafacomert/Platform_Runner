using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyController : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 5f;
    private Rigidbody rb;
    private float targetXPos;
    private float horizontalSpeed = 5f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        rb.MovePosition(new Vector3(Mathf.Lerp(transform.position.x, targetXPos, horizontalSpeed*Time.fixedDeltaTime), 
                        rb.velocity.y, 
                        transform.position.z + forwardSpeed * Time.fixedDeltaTime));
    }
}
