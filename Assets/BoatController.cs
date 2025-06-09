using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    private PlayerInputs _playerInputs;
    private Rigidbody _rb;

    public float acceleration = 15f;
    public float maxSpeed = 8f;
    public float turnSpeed = 50f;
    public float waterDrag = 0.98f;
    public float steeringInfluence = 0.5f; 
    public Transform pivotPoint; 
    
    private float _steerInput = 0f;
    private float _throttleInput = 0f;
    private Vector3 _currentVelocity;

    [SerializeField] private List<ParticleSystem> particles = new List<ParticleSystem>();
    [SerializeField] private ObjectRotator rotator;

    
    
    [Header("Cliff Detection")]
    [SerializeField] private int numberOfCliffChecks = 8;
    [SerializeField] private float cliffCheckDistance = 1.0f;
    [SerializeField] private float cliffMaxHeight = 1.5f;
    [SerializeField] private float cliffMaxCheckDistance = 2.0f;
    [SerializeField] private LayerMask cliffLayer;
    [SerializeField] private float cliffObstacleCheckDistance = 0.2f;
    [SerializeField] private LayerMask obstaclesLayer;
    [Range(0,1.0f)]
    [SerializeField] private float cliffCheckDot = 0.7f;
    [SerializeField] private float slopeCheckAngle = 45.0f;
    private List<Transform> _cliffCheckers;
    private int _lastCliffCheckCount;
    private float _lastCliffCheckDistance;
    
    
    public static BoatController Instance;
    
    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        ResetCliffCheckers();
        SetupCliffCheckers();

        _lastCliffCheckCount = numberOfCliffChecks;
        _lastCliffCheckDistance = cliffCheckDistance;
        _playerInputs = new PlayerInputs();
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void OnEnable()
    {
        _playerInputs.Enable();
        _playerInputs.Boat.Movement.performed += OnMoveBoatPerformed;
        _playerInputs.Boat.Movement.canceled += OnMoveBoatCancelled;
    }

    private void OnDisable()
    {
        _playerInputs.Boat.Movement.performed -= OnMoveBoatPerformed;
        _playerInputs.Boat.Movement.canceled -= OnMoveBoatCancelled;
        _playerInputs.Disable();
    }

    private void OnMoveBoatPerformed(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        _steerInput = input.x;
        _throttleInput = input.y;
    }

    private void OnMoveBoatCancelled(InputAction.CallbackContext context)
    {
        _steerInput = 0f;
        _throttleInput = 0f;
    }

    private void FixedUpdate()
    {
        if (_lastCliffCheckCount != numberOfCliffChecks || !Mathf.Approximately(_lastCliffCheckDistance, cliffCheckDistance))
        {
            ResetCliffCheckers();
            SetupCliffCheckers();
            _lastCliffCheckCount = numberOfCliffChecks;
            _lastCliffCheckDistance = cliffCheckDistance;
        }
        
        if (GameStateManager.Instance.GetGameState() != GameStates.PlayingBoat)
        {
            _steerInput = 0f;
            _throttleInput = 0f;
        }
        
        WaterFloatComponent floatComponent = GetComponent<WaterFloatComponent>();
        if (floatComponent != null && floatComponent.IsAnchored)
        {
            _steerInput = 0f;
            _throttleInput = 0f;
            return; // Skip MoveBoat
        }
        
        MoveBoat();
        BoatVfx();
        
    }
    
    private bool IsApproachingGround(Vector3 direction)
    {
        Vector3 moveDir = direction.normalized;

        foreach (Transform checker in _cliffCheckers)
        {
            Vector3 dirToChecker = (checker.position - transform.position).normalized;

            if (Vector3.Dot(moveDir, dirToChecker) > cliffCheckDot)
            {
                if (Physics.Raycast(checker.position, Vector3.down, out RaycastHit groundHit, cliffMaxCheckDistance, cliffLayer))
                {
                    float slopeAngle = Vector3.Angle(groundHit.normal, Vector3.up);

                    if (Physics.Raycast(checker.position, Vector3.down, out RaycastHit obstacleHit, cliffMaxCheckDistance, obstaclesLayer))
                    {
                        float distance = groundHit.distance - obstacleHit.distance;

                        if (distance > cliffObstacleCheckDistance)
                        {
                            return true; 
                        }
                    }

                    if (slopeAngle > slopeCheckAngle || groundHit.distance > cliffMaxHeight)
                    {
                        return true; 
                    }
                    return true;
                }
            }
        }

        return false;
    }
    
    private void ResetCliffCheckers()
    {
        if (_cliffCheckers != null)
        {
            foreach (Transform checker in _cliffCheckers)
            {
                if (checker != null)
                    Destroy(checker.gameObject);
            }
        }

        _cliffCheckers = new List<Transform>();
    }
    
    private void SetupCliffCheckers()
    {
        _cliffCheckers = new List<Transform>();
        float degrees = 360.0f / numberOfCliffChecks;
        for (int i = 0; i < numberOfCliffChecks; i++)
        {
            GameObject checker = new GameObject("BoatCliffChecker_" + i);
            checker.transform.parent = transform;
            checker.transform.localPosition = Quaternion.Euler(0, i * degrees, 0) * Vector3.forward * cliffCheckDistance;
            checker.hideFlags = HideFlags.HideInHierarchy;
            _cliffCheckers.Add(checker.transform);
        }
    }

    
    public void AnchorBoat(bool anchor)
    {
        Debug.Log("AnchorBoat: " + anchor);
        var floatComponent = GetComponent<WaterFloatComponent>();
        floatComponent.IsAnchored = anchor;

        if (anchor)
        {
            _currentVelocity = Vector3.zero;

           
            Vector3 v = _rb.linearVelocity;
            v.x = 0;
            v.z = 0;
            _rb.linearVelocity = v;
        }
    }
    
    private void MoveBoat()
    {
        Vector3 moveDirection = transform.forward * _throttleInput;
        
        if (IsApproachingGround(moveDirection))
        {
            _currentVelocity = Vector3.zero;
            _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);
            Debug.Log("Boat cliff ahead — movement cancelled");
            return;
        }
        
        // Update forward/backward velocity
        Vector3 targetVelocity = transform.forward * (_throttleInput * maxSpeed);
        _currentVelocity = Vector3.MoveTowards(_currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        // Apply drag when not throttling
        if (Mathf.Abs(_throttleInput) < 0.01f)
        {
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, Vector3.zero, (1f - waterDrag) * acceleration * Time.fixedDeltaTime);
        }

        // Apply velocity
        _rb.linearVelocity = new Vector3(_currentVelocity.x, _rb.linearVelocity.y, _currentVelocity.z);

        // Steering using pivot
        float speedFactor = _currentVelocity.magnitude * steeringInfluence;
        if (pivotPoint != null && speedFactor > 0.1f && Mathf.Abs(_steerInput) > 0.01f)
        {
            float direction = Mathf.Sign(Vector3.Dot(_currentVelocity, transform.forward));
            float turn = _steerInput * turnSpeed * direction * speedFactor * Time.fixedDeltaTime;

            // Rotate around pivot point
            Vector3 pivot = pivotPoint.position;
            Quaternion rotation = Quaternion.Euler(0f, turn, 0f);
            Vector3 dirFromPivot = transform.position - pivot;
            Vector3 rotatedDir = rotation * dirFromPivot;
            Vector3 newPos = pivot + rotatedDir;

            _rb.MovePosition(newPos);
            _rb.MoveRotation(_rb.rotation * rotation);
        }
    }

    private void BoatVfx()
    {
        if (_currentVelocity != Vector3.zero)
        {
            if (rotator.isActive)
            {
                return;
            }

            foreach (ParticleSystem s in particles)
            {
                s.Play();
            }
            rotator.isActive = true;
        }
        else
        {
            foreach (ParticleSystem s in particles)
            {
                s.Stop();
            }
            rotator.isActive = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (_cliffCheckers == null) return;

        Vector3 moveDirection = transform.forward * _throttleInput;
        Vector3 moveDir = moveDirection != Vector3.zero ? moveDirection.normalized : transform.forward;

        foreach (Transform checker in _cliffCheckers)
        {
            Vector3 dirToChecker = (checker.position - transform.position).normalized;
            bool withinThreshold = Vector3.Dot(moveDir, dirToChecker) > cliffCheckDot;

            // Perform both raycasts
            bool groundHit = Physics.Raycast(checker.position, Vector3.down, out RaycastHit hitGround, cliffMaxCheckDistance, cliffLayer);
            bool obstacleHit = Physics.Raycast(checker.position, Vector3.down, out RaycastHit hitObstacle, cliffMaxCheckDistance, obstaclesLayer);

            if (withinThreshold)
            {
                // Logic match: only block if both ground and obstacle are present, and obstacle is protruding
                if (groundHit && obstacleHit)
                {
                    float distance = hitGround.distance - hitObstacle.distance;
                    float slopeAngle = Vector3.Angle(hitGround.normal, Vector3.up);

                    if (distance > cliffObstacleCheckDistance || slopeAngle > slopeCheckAngle || hitGround.distance > cliffMaxHeight)
                    {
                        Gizmos.color = Color.red; // Blocked
                    }
                    else
                    {
                        Gizmos.color = Color.green; // Safe ground
                    }
                }
                else if (groundHit) // Only ground, still blocked
                {
                    Gizmos.color = new Color(1f, 0.5f, 0f); // Orange: warning
                }
                else
                {
                    Gizmos.color = Color.green; // Nothing below — open water
                }
            }
            else
            {
                Gizmos.color = new Color(1f, 1f, 1f, 0.3f); // Dim for outside dot range
            }

            Gizmos.DrawWireSphere(checker.position, 0.1f);
            Gizmos.DrawLine(checker.position, checker.position + Vector3.down * cliffMaxCheckDistance);

            // Optional: draw separate lines for ground and obstacle hits
            if (groundHit)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(checker.position, hitGround.point);
                Gizmos.DrawSphere(hitGround.point, 0.05f);
            }

            if (obstacleHit)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(checker.position, hitObstacle.point);
                Gizmos.DrawSphere(hitObstacle.point, 0.05f);
            }
        }
    }
}
