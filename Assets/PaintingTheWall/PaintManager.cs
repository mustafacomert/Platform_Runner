using UnityEngine;

public class PaintManager : MonoBehaviour
{
    [SerializeField] private GameObject brushPrefab;
    private Ray ray;
    private Camera mainCamera;
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
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider.CompareTag("Wall"))
                {
                    Instantiate(brushPrefab, hit.point + Vector3.forward * -0.1f, Quaternion.identity, transform);
                }
            }
        }
    }
}
