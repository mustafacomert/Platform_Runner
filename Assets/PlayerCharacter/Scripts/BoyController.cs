using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BoyController : MonoBehaviour
{
    //to make boy's forward speed constant and 16 unit per frame
    [SerializeField] private float forwardSpeed = 800f;
    [SerializeField] private float horizontalMoveDuration = 0.0003f;
    //constraint swerveAmount of the mouse, between this values
    [SerializeField] private float swerveAmount = 0.5f;
    //red cursor for painting the wall
    [SerializeField] private Texture2D paintCursor;
    //blue cursor for playing the game
    [SerializeField] private Texture2D defaultCursor;
    //show realtime rank of the player
    //will changed by oppenents 
    [SerializeField] private TextMeshProUGUI rankingBoard;
    [SerializeField] private GameObject wall;
    //x position of the mouse on the last frame
    private float lastXPos;
    //move amount of the mouse on the x-axis
    private float moveAmountX;
    private bool isMouseButtonHeldDown;

    private Rigidbody rb;
    private Animator animator;

    private Transform finishLine;
    //It will used by oppenents to decide whether, boy passed the finish line or not
    //aim is that is boy finished the game, oppenent script won't change the ranking board text
    public bool gameFinished;
   
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        finishLine = GameObject.FindGameObjectWithTag("FinishLine").transform;
        Cursor.SetCursor(defaultCursor, Vector3.zero, CursorMode.ForceSoftware);  
    }
    
    private void Update()
    {
        //if player clicks left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            lastXPos = Input.mousePosition.x;
        }
        //if left mouse button held down
        else if (Input.GetMouseButton(0))
        {
            moveAmountX = Input.mousePosition.x - lastXPos;
            lastXPos = Input.mousePosition.x;
            isMouseButtonHeldDown = true;
        }
        //if mouse button is released by player
        else if (Input.GetMouseButtonUp(0))
        {
            moveAmountX = 0f;
            isMouseButtonHeldDown = false;
        }
        //if game finished
        if(transform.position.z >= finishLine.position.z)
        {
            //player can't move boy any more
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
        animator.SetBool("isRunning", isMouseButtonHeldDown);
        //prevent boy from leaving the platform, make boy stay on the platform
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -6.3f, 6.3f),
                                         transform.position.y,
                                         transform.position.z);
    }

    private void FixedUpdate()
    {
        //constraint horizontal move amount of the boy
        moveAmountX = Mathf.Clamp(moveAmountX, -swerveAmount, swerveAmount);    
        //if the player held left mouse button down
        if(isMouseButtonHeldDown)
        {
            /*rb.MovePosition(new Vector3(Mathf.Lerp(transform.position.x, Mathf.Clamp(transform.position.x + moveAmountX, -6.5f, 6.5f), horizontalSpeed * Time.fixedDeltaTime), 
                            transform.position.y, 
                            transform.position.z + forwardSpeed * Time.fixedDeltaTime));*/
            
            //move forward(positive z-axis) with a constant speed(16 unit per frame)
            //and move horizontally with the constant speed(10 unit per framee)
            rb.velocity = new Vector3((moveAmountX / horizontalMoveDuration) * Time.fixedDeltaTime, 
                                       rb.velocity.y,  
                                       forwardSpeed * Time.fixedDeltaTime);
        }
        //if left mouse button is up, stop the boy
        else
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }
    }

    //if boy collides with an obstacle, restart the current scene
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Obstacle"))
        {
            Invoke("RestartScene", 0.2f);
        }
    }

    //restart the current scene
    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
