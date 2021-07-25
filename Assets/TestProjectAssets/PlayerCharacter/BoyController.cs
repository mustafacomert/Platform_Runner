using UnityEngine;
using UnityEngine.SceneManagement;

public class BoyController : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 800f;
    [SerializeField] private float horizontalMoveDuration = 0.0003f;
    [SerializeField] private float swerveAmount = 0.5f;
    [SerializeField] private Texture2D paintCursor;
    [SerializeField] private Texture2D defaultCursor;
    private float lastXPos;
    private float moveAmountX;
    private bool isMouseButtonHeldDown;

    private Rigidbody rb;
    private Animator animator;

    private Transform finishLine;

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        finishLine = GameObject.FindGameObjectWithTag("FinishLine").transform;
        Cursor.SetCursor(defaultCursor, Vector3.zero, CursorMode.ForceSoftware);
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
            isMouseButtonHeldDown = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("GetMouseButtonUp");
            moveAmountX = 0f;
            isMouseButtonHeldDown = false;
        }
       
        if(transform.position.z >= finishLine.position.z)
        {
            rb.velocity = Vector3.zero;
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, 0f, Time.deltaTime * 50f), transform.position.y, transform.position.z);
            animator.SetBool("isFinished", true);
            Cursor.SetCursor(paintCursor, Vector3.zero, CursorMode.ForceSoftware);
            if(Mathf.Abs(transform.position.x) <= 0.1f)
                this.enabled = false;
        }
        animator.SetBool("isRunning", isMouseButtonHeldDown);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -6.3f, 6.3f),
                                         transform.position.y,
                                         transform.position.z);
    }

    private void FixedUpdate()
    {
        moveAmountX = Mathf.Clamp(moveAmountX, -swerveAmount, swerveAmount);    
        if(isMouseButtonHeldDown)
        {
            /*rb.MovePosition(new Vector3(Mathf.Lerp(transform.position.x, Mathf.Clamp(transform.position.x + moveAmountX, -6.5f, 6.5f), horizontalSpeed * Time.fixedDeltaTime), 
                            transform.position.y, 
                            transform.position.z + forwardSpeed * Time.fixedDeltaTime));*/
            rb.velocity = new Vector3((moveAmountX / horizontalMoveDuration) * Time.fixedDeltaTime, rb.velocity.y,  forwardSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Obstacle"))
        {
            Invoke("RestartScene", 0.2f);
        }
    }
}
