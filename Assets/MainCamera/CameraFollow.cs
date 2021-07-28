using UnityEngine;

//make camera follow the boy, with the offset which obtained by scene, before running the game
public class CameraFollow : MonoBehaviour
{
    private Transform targetBoy;
    private Vector3 desiredPos;
    private float offsetZ;

    private void Awake()
    {
        desiredPos = transform.position;
        targetBoy = GameObject.FindGameObjectWithTag("Boy").transform;
        offsetZ = transform.position.z - targetBoy.position.z;
        offsetZ = Mathf.Abs(offsetZ);
    }
    private void LateUpdate()
    {
        desiredPos.z = targetBoy.position.z - offsetZ;
        transform.position = desiredPos; 
    }
}
