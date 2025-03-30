using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;


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
    
    private PlayerInputs _playerInputs;
    private Vector3 _playerMoveVector = Vector3.zero;
    private Rigidbody _rb;
    

    private void Awake()
    {
        _playerInputs = new PlayerInputs();
        _rb = GetComponent<Rigidbody>();
        
        //Need this to fix JITTERY Camera
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        
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
        InteractionManager.Instance.Interact();
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
        
        if (GameStateManager.Instance.GetGameState() != GameStates.Playing)
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
        
        if (isGrounded)
        {
            move = Vector3.ProjectOnPlane(move, _groundNormal);
            
            Vector3 downForce = _groundNormal * groundDownwardForce;
            _rb.linearVelocity = new Vector3(move.x, _rb.linearVelocity.y + downForce.y, move.z);
        }
        else
        {
            _rb.linearVelocity = new Vector3(move.x, _rb.linearVelocity.y, move.z);
        }
        
        if (_playerMoveVector == Vector3.zero)
        {
            _rb.linearVelocity = Vector3.zero;
            return;
        }
        LookInMovingDirection();
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
    }


}
