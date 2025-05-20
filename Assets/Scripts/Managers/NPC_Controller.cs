using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

//TODO: rename to NpcController
public class NPC_Controller : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private Vector3 currentTarget;
    [SerializeField] private Collider[] movementAreas;
    [SerializeField] private float wiggleRoom;
    private NavMeshAgent _aiAgent;

    [Header("IdleSettings")]
    [SerializeField] private bool isIdling;
    [SerializeField] private float minIdleTime;
    [SerializeField] private float maxIdleTime;

    private Coroutine _idleRoutine;
    [SerializeField] private bool canMove = true;
    [SerializeField] private float rotationSpeed = 3.0f;
    
    [SerializeField] private LayerMask avoideShit;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask groundLayer;
    
    
    
    private AnimationController _anim;
    public bool CanMove
    {
        get => canMove;
        set => canMove = value;
    }


    private void Start()
    {
        _aiAgent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<AnimationController>();
    }

    void Update()
    {
        _aiAgent.isStopped = !canMove;
        if (!canMove || isIdling)
        {
            _aiAgent.velocity = Vector3.zero;
            _anim.DisableAnimation(Animations.Move);
        }
        else
        {
            MoveTowardsTarget();
            _anim.EnableAnimation(Animations.Move);
        }      
    }

    private void MoveTowardsTarget()
    {
        if (isIdling)
        {
            return;
        }

        if (currentTarget == Vector3.zero)
        {
            Vector3 newDestination = GetPointInAreas();
            currentTarget = newDestination;
            _aiAgent.SetDestination(newDestination);
            _anim.EnableAnimation(Animations.Move);
            _aiAgent.isStopped = false;
        }

        if (DestinationReached())
        {
            currentTarget = Vector3.zero;
            _aiAgent.isStopped = true;
            _anim.DisableAnimation(Animations.Move);
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

    private Vector3 GetPointInAreas(int attempts = 0)
    {
        
        const int maxAttempts = 30;
        const float raycastHeight = 10f;

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Failed to find a valid point in movement areas after multiple attempts.");
            return transform.position;
        }
        
        Collider randomArea = movementAreas[Random.Range(0, movementAreas.Length)];
        float randomXValue = Random.Range(randomArea.bounds.min.x, randomArea.bounds.max.x);
        float randomZValue = Random.Range(randomArea.bounds.min.z, randomArea.bounds.max.z);
        
        Vector3 randomPoint = new Vector3(randomXValue, transform.position.y + raycastHeight, randomZValue);
        
        if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, 20f, groundLayer))
        {
            Vector3 point = hit.point;
            
            Collider[] hits = Physics.OverlapSphere(point, checkRadius, avoideShit);

            if (hits.Length > 0)
            {
                return GetPointInAreas(attempts + 1);
            }

            return hit.point;

        }
        else
        {
            return GetPointInAreas(attempts + 1);
        }
    }
    private void OnDrawGizmos()
    {
        
        if(Application.isPlaying && !isIdling)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(gameObject.transform.position, currentTarget);
        }
    }

    private IEnumerator IdleRoutine()
    {
        isIdling = true;
        _anim.DisableAnimation(Animations.Move);
        yield return new WaitForSeconds(Random.Range(minIdleTime, maxIdleTime));
        isIdling = false;
    }

    public void FacePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
