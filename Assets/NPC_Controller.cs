using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Controller : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private Vector3 currentTarget;
    [SerializeField] private Collider[] movementAreas;
    [SerializeField] private float wiggleRoom;
    private NavMeshAgent _aiAgent;

    [Header("IdleSettings")]
    [SerializeField] private bool isIdling;
    [SerializeField] private float maxIdleTime;

    private Coroutine _idleRoutine;

    private void Start()
    {
        _aiAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        if (isIdling)
        {
            return;
        }

        if(currentTarget == Vector3.zero)
        {
            Vector3 newDestination = GetPointInAreas();
            currentTarget = newDestination;
            _aiAgent.SetDestination(newDestination);
            _aiAgent.isStopped = false;
        }

        if (DestinationReached())
        {
            currentTarget = Vector3.zero;
            _aiAgent.isStopped = true;
            StartIdle();
        }
    }

    private bool DestinationReached()
    {
        float distance = Vector3.Distance(gameObject.transform.position, currentTarget);
        if(distance <= wiggleRoom)
        {
            return true;
        }
        return false;
    }

    private void StartIdle()
    {
        _idleRoutine = StartCoroutine(IdleRoutine());
    }

    private Vector3 GetPointInAreas()
    {
        Collider randomArea = movementAreas[Random.Range(0, movementAreas.Length)];
        float randomXValue = Random.Range(randomArea.bounds.min.x, randomArea.bounds.max.x);
        float randomZValue = Random.Range(randomArea.bounds.min.z, randomArea.bounds.max.z);

        return new Vector3(randomXValue, gameObject.transform.position.y, randomZValue);
    }
    private void OnDrawGizmos()
    {
        if(!isIdling)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(gameObject.transform.position, currentTarget);
        }
    }

    private IEnumerator IdleRoutine()
    {
        isIdling = true;
        yield return new WaitForSeconds(Random.Range(0, maxIdleTime));
        isIdling = false;
    }
}
