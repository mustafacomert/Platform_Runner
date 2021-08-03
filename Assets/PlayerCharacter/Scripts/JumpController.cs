using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpController : MonoBehaviour
{
    private bool jumpRequest;
    private bool isJumping;
    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            jumpRequest = true;
        }
    }
    private void FixedUpdate()
    {
        if(jumpRequest)
        {
            rb.AddForce(10f * Vector3.up, ForceMode.Impulse);
            jumpRequest = false;
        }
    }
}
