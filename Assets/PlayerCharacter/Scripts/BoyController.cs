﻿using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BoyController : MonoBehaviour
{
    //red cursor for painting the wall
    [SerializeField] private Texture2D paintCursor;
    //blue cursor for playing the game
    [SerializeField] private Texture2D defaultCursor;
    //show realtime rank of the player
    //will changed by oppenents 
    [SerializeField] private TextMeshProUGUI rankingBoard;
    [SerializeField] private GameObject wall;
    [SerializeField] private float speed = 100f;
    private Rigidbody rb;
    private Animator animator;
    private BoxCollider bc;
    private Transform finishLine;
    //Movement Variables
    private Vector3 dir;
    private float horizontal;
    private float vertical;
    private float targetAngle;
    //It will used by oppenents to decide whether, boy passed the finish line or not
    //aim is that is boy finished the game, oppenent script won't change the ranking board text
    public bool raceFinished { get; private set; }
    private bool isRunning;
    private bool isDead;

    //Jump Variables
    //................
    private bool jumpRequest;
    private bool heldJumpButton;
    private bool isJumping;
    private bool isGrounded;
    public bool isFalling;
    private float jumpTimeCounter;
    private float jumpTime = 0.15f;
    private float gravity = -50f;
    private float jumpSpeed = 3.2f;
    //ground check
    private float distToGround;
    private BoxCollider boxCollider;
    public LayerMask layerMask;
    public float maxDist;
   
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
        finishLine = GameObject.FindGameObjectWithTag("FinishLine").transform;
        Cursor.SetCursor(defaultCursor, Vector3.zero, CursorMode.ForceSoftware);
        boxCollider = GetComponent<BoxCollider>();
        distToGround = boxCollider.bounds.extents.y;
        Debug.Log("dist " + distToGround);
    }
    
   
    private void Update()
    {
        
        JumpInputCheck();
        MovemenentInputCheck();
        //if game finished
        if (transform.position.z >= finishLine.position.z)
        {
            MoveCharacterCenterOfFinishLine();
            if (!raceFinished)
            {
                AfterRaceFinished();
            }
            //if player is at center of x axis, disable this script
            DisableThisScript();
        }
        //if left mouse button is held down, set animator state to the running animation
        animator.SetBool("isRunning", isRunning);
    }


    private void FixedUpdate()
    {
        //GroundCheck();
        if (!isDead)
        {
            JumpTask();
            MovementTask();
        }
        AddGravity();
    }


    //if boy collides with an obstacle, restart the current scene
    private void OnCollisionStay(Collision collision)
    {

        if (collision.collider.CompareTag("Obstacle") )
        {
            isDead = true;
            rb.freezeRotation = false;
            animator.SetBool("isDead", true);
            Invoke("RestartScene", 1f);
        }
        if(collision.collider.CompareTag("Ground"))
        {
            animator.SetBool("isJumping", false);
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }




    //restart the current scene
    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    
    private void MovemenentInputCheck()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        dir = new Vector3(horizontal, 0f, vertical);
    }

    private void MovementTask()
    {
        if (dir.magnitude >= 0.1f)
        {
            isRunning = true;
            targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(targetAngle * Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, rot, Time.fixedDeltaTime * 10f);
            rb.velocity = dir * speed * Time.fixedDeltaTime + rb.velocity.y * Vector3.up;
        }
        else
        {
            rb.velocity = rb.velocity.y * Vector3.up;
            isRunning = false;
        }
    }

    private void JumpInputCheck()
    {
        if ((isGrounded || isFalling) && Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequest = true;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            heldJumpButton = true;
        }
        //falling
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            gravity = -65f;
            isJumping = false;
            isFalling = true;
        }
    }


    private void JumpTask()
    {
        if (isGrounded && rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }
        if (isGrounded && jumpRequest)
        {
            isJumping = true;
            animator.SetBool("isJumping", true);
            //reset counter
            jumpTimeCounter = jumpTime;
            rb.velocity += jumpSpeed * Vector3.up;
            jumpRequest = false;
        }
        if (heldJumpButton && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity += jumpSpeed * Vector3.up;
                jumpTimeCounter -= Time.fixedDeltaTime;
            }
            //falling
            else
            {
                gravity = -65f;
                isJumping = false;
            }
        }
    }


    private void AddGravity()
    {
        rb.velocity += gravity * Vector3.up * Time.fixedDeltaTime;
    }


    private void AfterRaceFinished()
    {
        //change animator state to victory animation
        animator.SetBool("isFinished", true);
        //show the wall
        wall.GetComponent<Animator>().SetBool("showWall", true);
        //make cursor red
        Cursor.SetCursor(paintCursor, Vector3.zero, CursorMode.ForceSoftware);
        //congrats text at the top of the screen
        string rank = rankingBoard.text.Split(' ')[0];
        var txt = "You Finished The Game at the position " + rank;
        rankingBoard.fontSize = 28;
        rankingBoard.text = txt;
        //prevent preceeding access of that block of code
        this.raceFinished = true;
    }


    private void MoveCharacterCenterOfFinishLine()
    {

        rb.velocity = Vector3.zero;
        //move boy to the center of x axis
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, 0f, Time.deltaTime * 50f), 0f, transform.position.z);
    }

    private void DisableThisScript()
    {
        //if player is at center of x axis, disable this script
        if (Mathf.Abs(transform.position.x) <= 0.1f)
            this.enabled = false;
    }

    private void GroundCheck()
    {
        /*Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.down));
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down), Color.red);
        RaycastHit hit;
        bool hitGroundBefore = false; 

        if(!isGrounded)
        {
            hitGroundBefore = false;
        }*/

        isGrounded = Physics.BoxCast(transform.position, bc.bounds.extents, Vector3.down, Quaternion.identity, maxDist, layerMask);
        Debug.Log("isGorund : " + isGrounded);
        /*if(isGrounded && !hitGroundBefore)
        {
            animator.SetBool("isJumping", false);
        }*/
    }
}
