using UnityEngine;
using System.Collections.Generic;

public class PaintManager : MonoBehaviour
{
    [SerializeField] private GameObject brushPrefab;
    [SerializeField] private GameObject wall;
    private Ray ray;
    private Camera mainCamera;
    private float minXValue;
    private float maxXValue;
    private float minYValue;
    private float maxYValue;

    private float clampedXPos;
    private float clampedYPos;

    private RaycastHit hit;
    private Dictionary<int, Vector3> paintedLocations;
    private Vector3 lastBrushPos;
    private int key = 0;
    private bool isMouseButtonUp = true;
    private Vector3 brushScale;

    private void Awake()
    {
        Vector3 wallPos = wall.transform.position;
        Vector3 wallScale = wall.transform.localScale;
        brushScale = brushPrefab.transform.localScale;

        minXValue = wallPos.x - wallScale.x / 2 + brushScale.x / 2;
        maxXValue = wallPos.x + wallScale.x / 2 - brushScale.x / 2;
        minYValue = wallPos.y - wallScale.y / 2 + brushScale.y / 2;
        maxYValue = wallPos.y + wallScale.y / 2 - brushScale.y / 2;

        mainCamera = Camera.main;
        paintedLocations = new Dictionary<int, Vector3>();
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

            Quaternion rotation = Quaternion.Euler(-90, 0, 0);
            if (Physics.Raycast(ray, out hit))
            {
                if (!hit.collider.CompareTag("Brush") && hit.collider.CompareTag("Wall"))
                {
                    clampedXPos = Mathf.Clamp(hit.point.x, minXValue, maxXValue);
                    clampedYPos = Mathf.Clamp(hit.point.y, minYValue, maxYValue);
                  
                    //this condition is for to maintain continous movement of the brush
                    if(!isMouseButtonUp && key != 0)
                    {
                        Vector3 tmp = new Vector3();
                        paintedLocations.TryGetValue(key-1, out tmp);
                   
                        clampedXPos = Mathf.Clamp(clampedXPos, tmp.x - brushScale.x, tmp.x + brushScale.x);
                        clampedYPos = Mathf.Clamp(clampedYPos, tmp.y - brushScale.y, tmp.y + brushScale.y);
                        
                    }

                    isMouseButtonUp = false;

                    lastBrushPos = new Vector3(clampedXPos, clampedYPos, hit.point.z) + 
                                   Vector3.forward * -0.1f;

                    //To prevent painting exact same location again.
                    if (!paintedLocations.ContainsValue(lastBrushPos))
                    {
                        paintedLocations.Add(key, lastBrushPos);
                        ++key;
                        Instantiate(brushPrefab, lastBrushPos, Quaternion.identity, transform);
                    }  
                }
            }
        }
    }
}
