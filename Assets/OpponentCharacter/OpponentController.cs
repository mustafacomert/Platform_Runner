using UnityEngine;
using UnityEngine.AI;
public class OpponentController : MonoBehaviour
{
    [SerializeField] private Transform destination;
    [SerializeField] private Transform spawnPoint;

    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        var x = Random.Range(-6.5f, 6.5f);
        Vector3 v = new Vector3(x, destination.position.y, destination.position.z);
        navMeshAgent.SetDestination(v);
        //navMeshAgent.updatePosition = false;
    }

    private void Update()
    {
        if(destination.position.z - transform.position.z < 0.6f)
        {
            Destroy(gameObject);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {

        if (collision.collider.CompareTag("Obstacle"))
        {
            Debug.Log("asd");
            navMeshAgent.nextPosition = spawnPoint.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
    }
}
