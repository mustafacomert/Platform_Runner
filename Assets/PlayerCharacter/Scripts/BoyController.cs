using UnityEngine;
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
    private Transform finishLine;
    private bool jumpRequest;
    private bool heldJump;
    private bool isJumping;
    private bool isGrounded;
    Vector3 dir;
    float horizontal;
    float vertical;
    float targetAngle;
    float angle;
    float vel;
    //It will used by oppenents to decide whether, boy passed the finish line or not
    //aim is that is boy finished the game, oppenent script won't change the ranking board text
    public bool gameFinished { get; set; }
    private bool isRunning;
    private float jumpTimeCounter;
    private float jumpTime = 0.12f;
    private float gravity = -50f;
    private float jumpSpeed = 3.2f;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        finishLine = GameObject.FindGameObjectWithTag("FinishLine").transform;
        Cursor.SetCursor(defaultCursor, Vector3.zero, CursorMode.ForceSoftware);
    }
    
    private void Update()
    {
        if(isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequest = true;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            heldJump = true;
        }
        else if(Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        dir = new Vector3(horizontal, 0f, vertical);

        //if game finished
        if (transform.position.z >= finishLine.position.z)
        {
            rb.velocity = Vector3.zero;
            //move boy to the center of x axis
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, 0f, Time.deltaTime * 50f), transform.position.y, transform.position.z);
            
            if (!gameFinished)
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
                gameFinished = true;
            }
            //if player is at center of x axis, disable this script
            if (Mathf.Abs(transform.position.x) <= 0.1f)
                this.enabled = false;
        }
        //if left mouse button is held down, set animator state to the running animation
        animator.SetBool("isRunning", isRunning);
    }

    private void FixedUpdate()
    {
        if (isGrounded && rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }
        if (isGrounded && jumpRequest)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            float jumpAngle = Mathf.Atan2(dir.z, dir.y) * Mathf.Rad2Deg;
            Debug.Log(jumpAngle);
            rb.rotation = Quaternion.Euler(jumpAngle, rb.rotation.y, 0f);
            rb.velocity += jumpSpeed * Vector3.up;
            jumpRequest = false;
        }
        if (heldJump && isJumping)
        {
            if(jumpTimeCounter > 0)
            {
                rb.velocity += jumpSpeed * Vector3.up;
                jumpTimeCounter -= Time.fixedDeltaTime;
            }
            else
            {
                Debug.Log("zero counrer");
                isJumping = false;
            }
        }
    
        if (dir.magnitude >= 0.1f)
        {
            isRunning = true;
            targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            //angle = Mathf.SmoothDampAngle(rb.rotation.y, targetAngle, ref vel, turnSmoothTime);
            rb.rotation = Quaternion.Euler(rb.rotation.x, targetAngle, 0f);
            rb.velocity = dir * speed * Time.fixedDeltaTime + rb.velocity.y * Vector3.up;
            
        }
        else
        {
            rb.velocity = rb.velocity.y * Vector3.up;
            isRunning = false;
        }
        rb.velocity += gravity * Vector3.up * Time.fixedDeltaTime;
    }

    //if boy collides with an obstacle, restart the current scene
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Obstacle"))
        {
            Invoke("RestartScene", 0.5f);
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
}
