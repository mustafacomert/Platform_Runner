using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyController : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 15f;
    [SerializeField] private float horizontalSpeed = 10f;
    [SerializeField] private float swerveAmount = 0.5f;

    private float lastXPos;
    private Rigidbody rb;
    private float moveAmountX;
    private bool isMouseButtonHeld; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    private void Update()
    {
        //if user clicks left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("GetMouseButtonDown");
            lastXPos = Input.mousePosition.x;
        }
        else if (Input.GetMouseButton(0))
        {
            Debug.Log("GetMouseButton");
            moveAmountX = Input.mousePosition.x - lastXPos;
            lastXPos = Input.mousePosition.x;
            isMouseButtonHeld = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("GetMouseButtonUp");
            moveAmountX = 0f;
            isMouseButtonHeld = false;
        }
    }
    
    private void FixedUpdate()
    {
        moveAmountX = Mathf.Clamp(moveAmountX, -swerveAmount, swerveAmount);    
        if(isMouseButtonHeld)
        {
            rb.MovePosition(new Vector3(Mathf.Lerp(transform.position.x, Mathf.Clamp(transform.position.x + moveAmountX, -6.5f, 6.5f), horizontalSpeed * Time.fixedDeltaTime), 
                            transform.position.y, 
                            transform.position.z + forwardSpeed * Time.fixedDeltaTime));
        }
    }
}
