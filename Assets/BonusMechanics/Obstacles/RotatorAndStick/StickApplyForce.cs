using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class StickApplyForce : MonoBehaviour
{
    private ConstantForce cnstForce;
    private int force = 40;
   
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Girl"))
        {
            OpponentController opponent = collision.gameObject.GetComponent<OpponentController>();
            NavMeshAgent nav = opponent.navMeshAgent;
            Rigidbody rb = collision.collider.attachedRigidbody;
            Vector3 dest = opponent.finishLine.position;
            Vector3 forceDir = collision.contacts[0].normal.normalized;
            nav.enabled = false;
            rb.isKinematic = false;
            
            StartCoroutine(ApplyForceAndReenableNavAgent(nav, rb, dest, forceDir));
        }
        if (collision.gameObject.CompareTag("Boy"))
        {
            Debug.Log(this.gameObject.name);
            Rigidbody rb = collision.collider.attachedRigidbody;
            Vector3 forceDir = collision.contacts[0].normal.normalized;
            rb.AddRelativeForce(forceDir * (force) * Time.fixedDeltaTime);
        }
    }

    private IEnumerator ApplyForceAndReenableNavAgent(NavMeshAgent nav, Rigidbody rb, Vector3 destination, Vector3 forceDir)
    {
        rb.AddForce(forceDir * (force)* Time.fixedDeltaTime);
        yield return new WaitForSeconds(1);
        rb.isKinematic = true;
        nav.enabled = true;
        nav.SetDestination(destination);
    }
}
