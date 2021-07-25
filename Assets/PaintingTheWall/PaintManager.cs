using UnityEngine;

public class PaintManager : MonoBehaviour
{
    [SerializeField] private GameObject brushPrefab;
    private Ray ray;
    private Camera mainCamera;
    private float minXValue = -6f;
    private float minYValue = 0.5f;
    private float maxYValue = 9.5f;
    private float clampedXPos;
    private float clampedYPos;
    private void Awake()
    {
        mainCamera = Camera.main;    
    }

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Quaternion rotation = Quaternion.Euler(-90, 0, 0);
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider.CompareTag("Wall"))
                {
                    clampedXPos = Mathf.Clamp(hit.point.x, minXValue, -minXValue);
                    clampedYPos = Mathf.Clamp(hit.point.y, minYValue, maxYValue);
                    hit.point = new Vector3(clampedXPos, clampedYPos, hit.point.z);
                    Instantiate(brushPrefab, hit.point + Vector3.forward * -0.1f, rotation, transform);
                }
            }
        }
    }
}
