using UnityEngine;

public class PaintManager : MonoBehaviour
{
    [SerializeField] private GameObject brushPrefab;
    [SerializeField] private GameObject wall;
    private Plane plane;
    private GameObject trail;
    private float distance;

    private void Awake()
    {
        MeshFilter filter = wall.GetComponent<MeshFilter>();
        Vector3 normal = new Vector3();

        if (filter && filter.mesh.normals.Length > 0)
            normal = filter.transform.TransformDirection(filter.mesh.normals[0]);

        plane = new Plane(normal.normalized, wall.transform.position);
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            
        }
        else if(Input.GetMouseButton(0))
        {
            //trail = (GameObject)Instantiate(brushPrefab, Input.mousePosition, Quaternion.identity);
            trail = (GameObject)Instantiate(brushPrefab, Input.mousePosition, Quaternion.identity);
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(mouseRay, out distance))
            {
                trail.transform.position = mouseRay.GetPoint(distance);
            }
        }
    }
}
