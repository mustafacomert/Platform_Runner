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
    private int opponentCount = 10;
    private BoyController boyController;

    private void Awake()
    {
        rank = opponentCount + 1;
        navMeshAgent = GetComponent<NavMeshAgent>();
        boy = GameObject.FindGameObjectWithTag("Boy").transform;
        boyController = boy.gameObject.GetComponent<BoyController>();
        var x = Random.Range(-6.5f, 6.5f);
        Vector3 v = new Vector3(x, destination.position.y, destination.position.z);
        navMeshAgent.SetDestination(v);
        //navMeshAgent.updatePosition = false;
    }

    private void Update()
    {
        if (!boyController.gameFinished)
        {
            if (isBehind)
            {
                if (boy.position.z < transform.position.z)
                {
                    isBehind = false;
                    ++rank;
                    txt.text = rank.ToString() + " / " + (opponentCount + 1).ToString();
                }
            }
            else
            {
                if (boy.position.z > transform.position.z)
                {
                    isBehind = true;
                    --rank;
                    txt.text = rank.ToString() + " / " + (opponentCount + 1).ToString();
                }
            }
        }

        if(destination.position.z - transform.position.z < 0.01f)
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
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("triigger");
            navMeshAgent.nextPosition = spawnPoint.position;
        }
    }
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
