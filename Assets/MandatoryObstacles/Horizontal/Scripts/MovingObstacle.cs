using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    private enum Directions
    {
        Up,
        Down,
        Left,
        Right
    }

    [SerializeField] private Directions dir;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveAmount = 6.5f;
    private Vector3 startingPos;
    private Rigidbody rb;
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
                targetPos = transform.position - Vector3.right * moveAmount;
                break;
            case Directions.Right:
                targetPos = transform.position + Vector3.right * moveAmount;
                break;
            case Directions.Up:
                targetPos = transform.position + Vector3.up * moveAmount;
                break;
            case Directions.Down:
                targetPos = transform.position - Vector3.up * moveAmount;
                break;
        }
        endingPos = targetPos;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(new Vector3(Mathf.Lerp(transform.position.x, targetPos.x, moveSpeed * Time.fixedDeltaTime),
                                    Mathf.Lerp(transform.position.y, targetPos.y, moveSpeed * Time.fixedDeltaTime),
                                    transform.position.z));
        //moves obstacle back and forth with the given direction and the given speed
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
