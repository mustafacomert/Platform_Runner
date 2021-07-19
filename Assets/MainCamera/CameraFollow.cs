using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform targetBoy;
    private Vector3 desiredPos;
    private float offsetZ;
   
    private void Awake()
    {
        offsetZ = 11f;
        desiredPos = transform.position;
        targetBoy = GameObject.FindGameObjectWithTag("Boy").transform;
       
    }
    private void LateUpdate()
    {
        desiredPos.z = targetBoy.position.z - offsetZ;
        transform.position = desiredPos; 
    }
}
