﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public enum Directions
    {
        Up,
        Down,
        Left,
        Right
    }

    private Rigidbody rb;
    [SerializeField] private Directions dir;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float moveAmount = 6.5f;
    private Vector3 startingPos;
    private Vector3 endingPos;
    private Vector3 targetPos;
    private bool isAtStartingPos;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startingPos = transform.position;
        isAtStartingPos = true;

        switch (dir)
        {
            case Directions.Left:
                targetPos = transform.position - new Vector3(moveAmount, 0, 0);
                break;
            case Directions.Right:
                targetPos = transform.position + new Vector3(moveAmount, 0, 0);
                break;
            case Directions.Up:
                targetPos = transform.position + new Vector3(0, moveAmount, 0);
                break;
            case Directions.Down:
                targetPos = transform.position - new Vector3(0, moveAmount, 0);
                break;
        }
        endingPos = targetPos;
    }

    private void FixedUpdate()
    {
        
        rb.MovePosition(new Vector3(Mathf.Lerp(transform.position.x, targetPos.x, moveSpeed * Time.fixedDeltaTime),
                                    transform.position.y,
                                    transform.position.z));
        if(Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            isAtStartingPos = !isAtStartingPos;
            if (!isAtStartingPos)
                targetPos = startingPos;
            else
                targetPos = endingPos;
        }
    }
}
