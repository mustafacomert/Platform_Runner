using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 40f;
    private enum Directions
    {
        left, right
    }
    
    [SerializeField] private Directions dir;
    private HashSet<Rigidbody> pickedUpObjs;
    private HashSet<GameObject> pickedUpBefore;
    private void Awake()
    {
        if(dir == Directions.left)
        {
            rotationSpeed = -rotationSpeed;
        }
        pickedUpObjs = new HashSet<Rigidbody>();
        pickedUpBefore = new HashSet<GameObject>();
    }
    private void Update()
    {
        transform.Rotate(Vector3.back * Time.deltaTime * rotationSpeed);       
    }

    private void FixedUpdate()
    {
        foreach (var t in pickedUpObjs)
        {
            GameObject go = t.gameObject;
            if (go.CompareTag("Girl") && !pickedUpBefore.Contains(go))
            {
                pickedUpBefore.Add(go);
                Debug.Log("ADASDDDDDAAD");
                OpponentController opponent = go.GetComponent<OpponentController>();
                NavMeshAgent nav = opponent.navMeshAgent;
                Rigidbody rb = go.GetComponent<Rigidbody>();
                Vector3 dest = opponent.finishLine.position;
                nav.enabled = false;
                rb.isKinematic = false;

                StartCoroutine(ApplyForceAndReenableNavAgent(nav, rb, dest));
            }
            else if(t.gameObject.CompareTag("Boy"))
            {
                //t.position += Vector3.right * 4f * Time.deltaTime;
                t.AddForce(Vector3.right * (8 * rotationSpeed) * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider c = collision.collider;
        if (c.CompareTag("Boy") || c.CompareTag("Girl"))
        {
            pickedUpObjs.Add(c.attachedRigidbody);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Collider c = collision.collider;
        if (c.CompareTag("Boy") || c.CompareTag("Girl"))
        {
            pickedUpObjs.Remove(c.gameObject.GetComponent<Rigidbody>());
        }
    }

    private IEnumerator ApplyForceAndReenableNavAgent(NavMeshAgent nav, Rigidbody rb, Vector3 destination)
    {
        rb.AddForce(Vector3.right * (rotationSpeed*8) * Time.fixedDeltaTime, ForceMode.VelocityChange);
        yield return new WaitForSeconds(0.5f);
        rb.isKinematic = true;
        nav.enabled = true;
        nav.SetDestination(destination);
    }

}
