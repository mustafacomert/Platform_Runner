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

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        finishLine = GameObject.FindGameObjectWithTag("FinishLine").transform;
        Cursor.SetCursor(defaultCursor, Vector3.zero, CursorMode.ForceSoftware); 
        
    }
    
    private void Update()
    {
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
        if (dir.magnitude >= 0.1f)
        {
            isRunning = true;
            targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            //angle = Mathf.SmoothDampAngle(rb.rotation.y, targetAngle, ref vel, turnSmoothTime);
            rb.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            rb.velocity = dir * speed * Time.fixedDeltaTime;
        }
        else
        {
            rb.velocity = Vector3.zero;
            isRunning = false;
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
