using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BoyController : MonoBehaviour
{
    //cursor variables
    private Texture2D redCursor;
    private Texture2D blueCursor;

    //show realtime rank of the player
    //will changed by oppenents 
    //GameObjects from scene
    private TextMeshProUGUI rankingBoard;
    //wall to be shown after race finished
    private GameObject wall;
    private Transform finishLine;

    //components
    private Rigidbody rb;
    private Animator animator;

    //Movement Variables
    [SerializeField] private float speed = 800;
    private Vector3 dir;
    private float horizontal;
    private float vertical;
    private float targetAngle;
    //It will used by oppenents to decide whether, boy passed the finish line or not
    //aim is that is boy finished the game, oppenent script won't change the ranking board text

    //State variables
    public bool raceFinished { get; private set; }
    private bool isRunning;
    private bool isDead;
    private bool isJumping;
    private bool isFalling;
    private bool isGrounded;

    //Jump Variables
    private bool jumpRequest;
    private bool heldJumpButton;
    private float jumpTimeCounter;
    private float jumpTime = 0.1f;
    //boy's rigidbody doesnt use gravity from physics engine
    //we will apply gravity force manually
    private float gravity = -50f;
    [SerializeField] private float jumpSpeed = 120;

    private void Awake()
    {
        //Init. component variables
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        //Init. gameObject with the help of their tags 
        finishLine = GameObject.FindGameObjectWithTag("FinishLine").transform;
        rankingBoard = GameObject.FindGameObjectWithTag("RankingBoard").GetComponent<TextMeshProUGUI>();
        wall = GameObject.FindGameObjectWithTag("Wall");
        //Init Cursors from project folders
        blueCursor = Resources.Load<Texture2D>("Cursors/blue");
        redCursor = Resources.Load<Texture2D>("Cursors/red");
        //set cursor to blue cursor
        Cursor.SetCursor(blueCursor, Vector3.zero, CursorMode.ForceSoftware);
    }


    private void Update()
    {
        //check user input for jump movement
        JumpInputCheck();
        //check user input for horizontal and back/forward movement
        MovemenentInputCheck();
        //condition to check whether game finished or not
        if (transform.position.z >= finishLine.position.z)
        {
            //move character at the center of the finish line, and froze its velocity on all axis
            MoveCharacterCenterOfFinishLine();
            if (!raceFinished)
            {
                AfterRaceFinished();
            }
            //if player is at center of x axis, disable this script
            DisableThisScript();
        }
    }


    private void FixedUpdate()
    {
        animator.SetBool("isJumping", isJumping || isFalling);
        animator.SetBool("isDead", isDead);
        animator.SetBool("isRunning", isRunning);
        JumpStateTracker();
        if (!isDead)
        {
            JumpTask();
            MovementTask();
        }
        if(!isGrounded)
            AddGravity();
    }


    //if boy collides with an obstacle, restart the current scene
    private void OnCollisionStay(Collision collision)
    {

        if (collision.collider.CompareTag("Obstacle"))
        {
            isDead = true;
            rb.freezeRotation = false;
            Invoke("RestartScene", 1f);
        }
        if (collision.collider.CompareTag("Ground"))
        {
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
        if((isGrounded ||isFalling) && Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequest = true;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            heldJumpButton = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            heldJumpButton = false;
        }
    }


    private void JumpTask()
    {
        if (isGrounded && jumpRequest)
        {
            jumpRequest = false;
            isJumping = true;
            //reset counter
            jumpTimeCounter = jumpTime;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.velocity += Vector3.up * jumpSpeed*4f * Time.deltaTime;
        }
       
        if (heldJumpButton && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity += Vector3.up * jumpSpeed * Time.deltaTime;
                jumpTimeCounter -= Time.fixedDeltaTime;
            }
            else
            {
                heldJumpButton = false;
            }
        }
       
    }


    private void AddGravity()
    {
        if (isJumping)
            gravity = -65f;
        if (isFalling)
            gravity = -75f;

        rb.velocity += gravity * Vector3.up * Time.fixedDeltaTime;
    }

    private void JumpStateTracker()
    {
        if (isJumping && rb.velocity.y < -0.15f)
        {
            isFalling = true;
            isJumping = false;
        }

        if (isFalling && rb.velocity.y > -0.1f)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            isFalling = false;
            isJumping = false;
        }
    }
    private void AfterRaceFinished()
    {
        //change animator state to victory animation
        animator.SetBool("isFinished", true);
        //show the wall
        wall.transform.parent.GetComponent<Animator>().SetBool("showWall", true);
        //make cursor red
        Cursor.SetCursor(redCursor, Vector3.zero, CursorMode.ForceSoftware);
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
        //froze character
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

    /*private void GroundCheck()
    {

    //ground check
     float distToGround;
     BoxCollider boxCollider;
     LayerMask layerMask;
     float maxDist;
    Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.down));
    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down), Color.red);
    RaycastHit hit;
    bool hitGroundBefore = false; 

    if(!isGrounded)
    {
        hitGroundBefore = false;
    }

    isGrounded = Physics.BoxCast(transform.position, bc.bounds.extents, Vector3.down, Quaternion.identity, maxDist, layerMask);
        Debug.Log("isGorund : " + isGrounded);
        /*if(isGrounded && !hitGroundBefore)
        {
            animator.SetBool("isJumping", false);
        }
    }*/
}
