using UnityEngine;

public class PaintManager : MonoBehaviour
{
    //red cube to instantiated on the wall to mimic, painting effect on the wall
    [SerializeField] private GameObject brushPrefab;
    //wall which will be painted
    [SerializeField] private GameObject wall;
    private Ray ray;
    private Camera mainCamera;
    //boundary position of the wall that will be painted
    private float minXValue;
    private float maxXValue;
    private float minYValue;
    private float maxYValue;
    //x position between minXValue and maxXValue
    private float clampedXPos;
    //y position between minYValue and maxYValue
    private float clampedYPos;
    //used for determining the hit point of the mouse on the world space
    private RaycastHit hit;
    //variable to supply continous painting on the wall
    //prevents space between red cubes(brushes)
    private Vector3 lastBrushPos;
    private bool isFirstTime = true;
    private bool isMouseButtonUp = true;
    //to make brushScale parameterized
    private Vector3 brushScale;

    private void Awake()
    {
        Vector3 wallPos = wall.transform.position;
        wallPos = wallPos - 2 * wallPos.y * Vector3.up;
        Debug.Log(wallPos);
        Vector3 wallScale = wall.transform.localScale;
        brushScale = brushPrefab.transform.localScale;

        minXValue = wallPos.x - wallScale.x / 2 + brushScale.x / 2;
        maxXValue = wallPos.x + wallScale.x / 2 - brushScale.x / 2;
        minYValue = wallPos.y - wallScale.y / 2 + brushScale.y / 2;
        maxYValue = wallPos.y + wallScale.y / 2 - brushScale.y / 2;

        mainCamera = Camera.main;
    }
    private void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            isMouseButtonUp = true;
        }

        if (Input.GetMouseButton(0))
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                //if mouse hits the wall on world space
                if (hit.collider.CompareTag("Wall"))
                {
                    clampedXPos = Mathf.Clamp(hit.point.x, minXValue, maxXValue);
                    clampedYPos = Mathf.Clamp(hit.point.y, minYValue, maxYValue);
                  
                    //this condition is to maintain continuous movement of the brush
                    //if it's first brush don't need to clamp
                    if(!isMouseButtonUp && !isFirstTime)
                    {
                        clampedXPos = Mathf.Clamp(clampedXPos, 
                                                  lastBrushPos.x - brushScale.x, 
                                                  lastBrushPos.x + brushScale.x);
                        clampedYPos = Mathf.Clamp(clampedYPos,                 
                                                  lastBrushPos.y - brushScale.y, 
                                                  lastBrushPos.y + brushScale.y);
                    }
                    isMouseButtonUp = false;
                    lastBrushPos = new Vector3(clampedXPos, clampedYPos, hit.point.z) + 
                                   Vector3.forward * -0.1f;
                    Instantiate(brushPrefab, lastBrushPos, Quaternion.identity, transform);
                    isFirstTime = false;
                }
            }
        }
    }
}
