using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 40f;
    private enum Directions
    {
        left, right
    }
    
    [SerializeField] private Directions dir;
    private HashSet<Rigidbody> pickedUpObjs;
    private void Awake()
    {
        if(dir == Directions.left)
        {
            rotationSpeed = -rotationSpeed;
        }
        pickedUpObjs = new HashSet<Rigidbody>();
    }
    private void Update()
    {
        transform.Rotate(Vector3.back * Time.deltaTime * rotationSpeed);       
    }

    private void FixedUpdate()
    {
        foreach (var t in pickedUpObjs)
        {
            Debug.Log("t : " + t.name);
            //t.position += Vector3.right * 4f * Time.deltaTime;
            t.AddForce(Vector3.right * (8*rotationSpeed) * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider c = collision.collider;
        if (c.CompareTag("Boy") || c.CompareTag("Girl"))
        {
            pickedUpObjs.Add(c.gameObject.GetComponent<Rigidbody>());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Collider c = collision.collider;
        if (c.CompareTag("Boy") || c.CompareTag("Girl"))
        {
            pickedUpObjs.Remove(c.gameObject.GetComponent<Rigidbody>());
            c.attachedRigidbody.WakeUp();
        }
    }

}
