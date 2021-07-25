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
                //!hit.collider.CompareTag("Brush") && 
                if (hit.collider.CompareTag("Wall"))
                {
                    clampedXPos = Mathf.Clamp(hit.point.x, minXValue, maxXValue);
                    clampedYPos = Mathf.Clamp(hit.point.y, minYValue, maxYValue);
                  
                    //this condition is to maintain continuous movement of the brush
                    if(!isMouseButtonUp && key != 0)
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
                    key = 1;
                }
            }
        }
    }
}
