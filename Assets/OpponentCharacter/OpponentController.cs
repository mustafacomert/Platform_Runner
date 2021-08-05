using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.SceneManagement;

public class OpponentController : MonoBehaviour
{
    [SerializeField] private Transform destination;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private TextMeshProUGUI txt;

    private Transform boy;
    private NavMeshAgent navMeshAgent;

    private bool isBehind;
    private static int rank;
    private int opponentCount;
    private BoyController boyController;

    private void Awake()
    {
        //how many oppenents are there 
        opponentCount = transform.parent.childCount;
        Debug.Log("soo" + opponentCount);
        rank = opponentCount + 1;
        txt.text = rank.ToString() + " / " + rank.ToString();
        navMeshAgent = GetComponent<NavMeshAgent>();
        boy = GameObject.FindGameObjectWithTag("Boy").transform;
        boyController = boy.gameObject.GetComponent<BoyController>();
        var x = Random.Range(0, 10000);
        if(x % 2 == 0)
        {
            x = x % 6;
        }
        else
        {
            x = (x % 6) * -1;
        }
        Vector3 v = new Vector3(x, destination.position.y, destination.position.z);
        navMeshAgent.SetDestination(v);
        //navMeshAgent.updatePosition = false;
    }

    private void Update()
    {
        //change the ranking of the boy
        if (!boyController.raceFinished)
        {
            
            if (isBehind)
            {
                //if oppenent is behind, check if this object pass the boy or not
                if (boy.position.z < transform.position.z)
                {
                    //if it passed, increase the rank of the boy and change the ranking board w.r.t that fact
                    isBehind = false;
                    ++rank;
                    txt.text = rank.ToString() + " / " + (opponentCount + 1).ToString();
                }
            }
            else
            {
                //if oppenent is front of the boy, check if boy pass this object or not
                if (boy.position.z > transform.position.z)
                {
                    //if boy passed, decrease the rank of the boy and change the ranking board w.r.t that fact
                    isBehind = true;
                    --rank;
                    txt.text = rank.ToString() + " / " + (opponentCount + 1).ToString();
                }
            }
        }
        //if this oppenent pass the finish line, destroy it
        if(destination.position.z - transform.position.z < 0.01f)
        {
            Destroy(gameObject);
        }
    }

    //if opponent hits a obtacle, send her to the starting position
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            navMeshAgent.enabled = false;
            transform.position = spawnPoint.position;
            navMeshAgent.enabled = true;
            var x = Random.Range(-6.5f, 6.5f);
            Vector3 v = new Vector3(x, destination.position.y, destination.position.z);
            navMeshAgent.SetDestination(v);
        }
    }

    //below three function used for setting, static variable, on scene load
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        rank = opponentCount + 1;
    }

}
