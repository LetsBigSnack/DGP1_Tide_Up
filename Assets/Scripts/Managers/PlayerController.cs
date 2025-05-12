using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using static AnimationController;

public class PlayerController : MonoBehaviour
{
    
    
    [Header("Rotation")]
    [SerializeField] private float rotateStepSpeed = 500;
    
    [Header("Sprint")]
    [SerializeField] private bool isSprinting = true;
    private const float DefaultMultiplier = 1.0f;
    [SerializeField] private float currentMultiplier = 1.0f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float sprintTimeStep = 0.1f;
    
    [Header("Ground Check")]    
    [SerializeField] private bool isGrounded = true;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float groundCheckMaxDistance = 1.0f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float fallFactor = 0.9f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundThreshold = 0.5f;
    [SerializeField] private float thresholdEpsilon = 0.0001f;
    [SerializeField]private float groundGraceTime = 0.1f;
    [SerializeField] private float groundDownwardForce = -0.5f;
    [SerializeField] private float timeSinceLastGrounded;
    [SerializeField] private ForceMode forceMode = ForceMode.Acceleration;
    private Vector3 _raycastHitPoint;
    private Vector3 _groundNormal = Vector3.up;
    
    
    [Header("Cliff Detection")]
    [SerializeField] private int numberOfCliffChecks = 8;
    [SerializeField] private float cliffCheckDistance = 1.0f;
    [SerializeField] private float cliffMaxHeight = 1.5f;
    [SerializeField] private float cliffMaxCheckDistance = 2.0f;
    [SerializeField] private LayerMask cliffLayer;
    [Range(0,1.0f)]
    [SerializeField] private float cliffCheckDot = 0.7f;
    [SerializeField] private float slopeCheckAngle = 45.0f;
    private List<Transform> _cliffCheckers;
    private int _lastCliffCheckCount;
    private float _lastCliffCheckDistance;
    
    
    private PlayerInputs _playerInputs;
    private Vector3 _playerMoveVector = Vector3.zero;
    private Rigidbody _rb;
    private bool playerCanMove = true;
    private AnimationController _anim;
    

    
    
    
    private void Awake()
    {
        _anim = GetComponent<AnimationController>();

        _playerInputs = new PlayerInputs();
        _rb = GetComponent<Rigidbody>();
        
        //Need this to fix JITTERY Camera
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        
        ResetCliffCheckers();
        SetupCliffCheckers();
        _lastCliffCheckCount = numberOfCliffChecks;
        _lastCliffCheckDistance = cliffCheckDistance;
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
            GameObject checker = new GameObject("CliffChecker_" + i);
            checker.transform.parent = transform;
            checker.transform.localPosition = Quaternion.Euler(0, i * degrees, 0) * Vector3.forward * cliffCheckDistance;
            checker.hideFlags = HideFlags.HideInHierarchy;
            _cliffCheckers.Add(checker.transform);
        }
    }

    
    private void OnEnable()
    {
        _playerInputs.Enable();
        
        //Movement
        _playerInputs.Player.Movement.Enable();
        _playerInputs.Player.Movement.performed += OnMovePlayerPreformed;
        _playerInputs.Player.Movement.canceled += OnMovePlayerCancelled;
        
        //Sprint
        _playerInputs.Player.Sprint.Enable();
        _playerInputs.Player.Sprint.performed += ToggleSprint;
        
        //Interact
        _playerInputs.Player.Interact.Enable();
        _playerInputs.Player.Interact.performed += Interact;

    }



    private void OnDisable()
    {
        _playerInputs.Disable();
        
        //Movement
        _playerInputs.Player.Movement.Disable();
        _playerInputs.Player.Movement.performed -= OnMovePlayerPreformed;
        _playerInputs.Player.Movement.canceled -= OnMovePlayerCancelled;
        
        //Sprint
        _playerInputs.Player.Sprint.Disable();
        _playerInputs.Player.Sprint.performed -= ToggleSprint;
        
        //Interact
        _playerInputs.Player.Interact.Disable();
        _playerInputs.Player.Interact.performed -= Interact;
        
    }

    private void ToggleSprint(InputAction.CallbackContext value)
    {
        isSprinting = !isSprinting;
    }
    
    private void Interact(InputAction.CallbackContext value)
    {
        if (GameStateManager.Instance.GetGameState() == GameStates.MiniGame)
        {
            return;
        }
        else
        {
            if( InteractionManager.Instance.ReturnInteractableType() == Data.InteractableType.Pickup)
            {
                playerCanMove = false;
                _anim.EnableAnimation(Animations.Pick);
                return;
            }

            InteractionManager.Instance.Interact();
        }
    }

    public void DelayedInteract()
    {
        InteractionManager.Instance.Interact();
    }

    public void SetPlayerCanMove()
    {
        playerCanMove = true;
    }

    private void OnMovePlayerPreformed(InputAction.CallbackContext value)
    {
        Vector2 axis = value.ReadValue<Vector2>();
        _playerMoveVector = new Vector3(axis.x, 0, axis.y);
    }

    private void OnMovePlayerCancelled(InputAction.CallbackContext value)
    {
        _playerMoveVector = Vector3.zero;
    }
    
    
    private void FixedUpdate()
    {
        
        if (_lastCliffCheckCount != numberOfCliffChecks  || !Mathf.Approximately(_lastCliffCheckDistance, cliffCheckDistance))
        {
            ResetCliffCheckers();
            SetupCliffCheckers();
            _lastCliffCheckCount = numberOfCliffChecks;
        }

        if (GameStateManager.Instance.GetGameState() != GameStates.PlayingCharacter || !playerCanMove)
        { 
            _playerMoveVector = Vector3.zero;
        }
        
        bool actuallyOnGround = CheckForGround();

        if (actuallyOnGround)
        {
            isGrounded = true;
            timeSinceLastGrounded = 0f;
        }
        else
        {
            timeSinceLastGrounded += Time.deltaTime;
            if (timeSinceLastGrounded >= groundGraceTime)
            {
                isGrounded = false;
            }
        }

        ApplySprint();
        MovePlayer();
        ApplyFall();
    }

    private void ApplySprint()
    {
        if (!isSprinting || _playerMoveVector == Vector3.zero)
        {
            currentMultiplier = DefaultMultiplier;
            return;
        }
        
        currentMultiplier = Mathf.Lerp(currentMultiplier, sprintMultiplier, Time.deltaTime * sprintTimeStep);
    }
    
    private void MovePlayer()
    {
        Vector3 move = _playerMoveVector * (Player.Instance.MoveSpeed * currentMultiplier);
        
        
        if (IsApproachingCliff(move.normalized))
        {
            _rb.linearVelocity = Vector3.zero;
            Debug.Log("Cliff ahead — movement cancelled");
            return;
        }
        

        if (playerCanMove)
        {
            if (isGrounded)
            {
                move = Vector3.ProjectOnPlane(move, _groundNormal);

                Vector3 downForce = _groundNormal * groundDownwardForce;
                _rb.linearVelocity = new Vector3(move.x, _rb.linearVelocity.y + downForce.y, move.z);
                _anim.EnableAnimation(Animations.Move);
            }
            else
            {
                _rb.linearVelocity = new Vector3(move.x, _rb.linearVelocity.y, move.z);
            }

        }

        _anim.Velocity = currentMultiplier;

        if (_playerMoveVector == Vector3.zero )
        {
            _rb.linearVelocity = Vector3.zero;
            _anim.DisableAnimation(Animations.Move);
            return;
        }
        LookInMovingDirection();
    }
    
    private bool IsApproachingCliff(Vector3 direction)
    {
        Vector3 moveDir = direction.normalized;

        foreach (Transform checker in _cliffCheckers)
        {
            Vector3 dirToChecker = (checker.position - transform.position).normalized;
            if (Vector3.Dot(moveDir, dirToChecker) > cliffCheckDot)
            {
                if (Physics.Raycast(checker.position, Vector3.down, out RaycastHit hit, cliffMaxCheckDistance, cliffLayer))
                {
                    float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                    
                    if (slopeAngle > slopeCheckAngle || hit.distance > cliffMaxHeight)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        return false;
    }
    private void LookInMovingDirection()
    {
        Quaternion toRotation = Quaternion.LookRotation(_playerMoveVector, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotateStepSpeed * Time.deltaTime);
    }
    
    private bool CheckForGround()
    {
        RaycastHit hit;
        bool raycastSuccess = Physics.SphereCast(groundCheck.position, groundCheckRadius, transform.up * -1, out hit, groundCheckMaxDistance, groundLayer);
            
        if (raycastSuccess && hit.collider.gameObject.CompareTag("Ground") && hit.distance <= groundThreshold + thresholdEpsilon)
        {
            _raycastHitPoint = hit.point;
            _groundNormal = hit.normal;
            return true;
        }
        else
        {
            _raycastHitPoint = Vector3.zero;
            _groundNormal = Vector3.up;
            return false;
        }
    }
    
    private void ApplyFall()
    {
        if (!isGrounded)
        {
            _rb.AddForce(Vector3.up * (-1 * fallFactor), forceMode);
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!groundCheck) return; // safety check if groundCheck isn't assigned
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(_raycastHitPoint, _groundNormal * 10f);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, Vector3.up * (-1 * fallFactor) * 10f);
        
        
        // 1) Draw the start sphere at groundCheck
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        // 2) Draw the end sphere at the farthest distance
        Vector3 sphereCastEnd = groundCheck.position + Vector3.down * groundCheckMaxDistance;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(sphereCastEnd, groundCheckRadius);
    
        // 3) Draw a line between these two spheres
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(groundCheck.position, sphereCastEnd);

        // 4) Draw a sphere (or line) at (groundThreshold + thresholdEpsilon)
        float checkDistance = groundThreshold + thresholdEpsilon;
        if (checkDistance < groundCheckMaxDistance) // So we can see it clearly
        {
            Gizmos.color = Color.magenta;
            Vector3 thresholdPoint = groundCheck.position + Vector3.down * checkDistance;
            Gizmos.DrawWireSphere(thresholdPoint, groundCheckRadius);

            // Optionally draw another line from start to threshold
            Gizmos.DrawLine(groundCheck.position, thresholdPoint);
        }
    
        // 5) If we’re grounded, visualize the actual hit point
        if (isGrounded)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_raycastHitPoint, 0.1f);
        }
        
        
        if (_cliffCheckers == null) return;

        Vector3 moveDir = _playerMoveVector != Vector3.zero ? _playerMoveVector.normalized : transform.forward;

        foreach (Transform checker in _cliffCheckers)
        {
            Vector3 dirToChecker = (checker.position - transform.position).normalized;
            bool withinThreshold = Vector3.Dot(moveDir, dirToChecker) > cliffCheckDot;

            
            bool hit = Physics.Raycast(checker.position, Vector3.down, out RaycastHit hitInfo, cliffMaxCheckDistance, cliffLayer);

            
            if (withinThreshold)
            {
                Gizmos.color = hit && hitInfo.distance <= cliffMaxHeight ? Color.green : Color.red;
            }
            else
            {
                Gizmos.color = new Color(1f, 1f, 1f, 0.3f); // transparent white
            }

            Gizmos.DrawWireSphere(checker.position, 0.1f);
            Gizmos.DrawLine(checker.position, checker.position + Vector3.down * cliffMaxCheckDistance);
            
            Gizmos.color = Color.yellow;
            
            Gizmos.DrawLine(checker.position, checker.position + Vector3.down * cliffMaxHeight);
            
            if (hit)
            {
                float slopeAngle = Vector3.Angle(hitInfo.normal, Vector3.up);
                
                Gizmos.color = slopeAngle > slopeCheckAngle ? Color.red : Color.green;
                Gizmos.DrawLine(hitInfo.point, hitInfo.point + hitInfo.normal * 0.5f);

                Gizmos.DrawSphere(hitInfo.point, 0.05f);
            }
        }
    }
    


}
