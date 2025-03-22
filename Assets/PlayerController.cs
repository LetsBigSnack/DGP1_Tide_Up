using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private bool isGrounded = true;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float fallFactor = 0.9f;
    private Vector3 _groundNormal = Vector3.up;
    [SerializeField] private float rotateStepSpeed = 500;
    [SerializeField] private LayerMask groundLayer;

    
    private PlayerInputs _playerInputs;
    private Vector3 _playerMoveVector = Vector3.zero;
    private Rigidbody _rb;
    private Vector3 _raycastHitPoint;

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
        

    }
    
    private void OnDisable()
    {
        _playerInputs.Disable();
        
        //Movement
        _playerInputs.Player.Movement.Disable();
        _playerInputs.Player.Movement.performed -= OnMovePlayerPreformed;
        _playerInputs.Player.Movement.canceled -= OnMovePlayerCancelled;
        

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
    
    void Start()
    {
        StartCoroutine(CheckForGround());
    }
    
    private void FixedUpdate()
    {
        
        if (GameStateManager.Instance.GetGameState() != GameStates.Playing)
        { 
            _playerMoveVector = Vector3.zero;
        }
        
        MovePlayer();
        ApplyFall();
    }
    
    private void MovePlayer()
    {
        Vector3 move = _playerMoveVector * Player.Instance.MoveSpeed;
        if (isGrounded)
        {
            move = Vector3.ProjectOnPlane(move, _groundNormal);
        }
        
        _rb.linearVelocity = new Vector3(move.x, _rb.linearVelocity.y, move.z);
        
        if (_playerMoveVector == Vector3.zero)
        {
            return;
        }
        LookInMovingDirection();
    }
    
    private void LookInMovingDirection()
    {
        Quaternion toRotation = Quaternion.LookRotation(_playerMoveVector, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotateStepSpeed * Time.deltaTime);
    }
    
    private IEnumerator CheckForGround()
    {
        RaycastHit hit;
        
        while (true)
        {
            bool raycastSuccess = Physics.SphereCast(groundCheck.position, groundCheckRadius, transform.up * -1, out hit, groundCheckRadius + 0.1f, groundLayer);
            if (raycastSuccess && hit.collider.gameObject.CompareTag("Ground") && hit.distance <= 0.50001f)
            {
                isGrounded = true;
                _raycastHitPoint = hit.point;
                _groundNormal = hit.normal;
            }
            else
            {
                isGrounded = false;
                _raycastHitPoint = Vector3.zero;
                _groundNormal = Vector3.up;
            }
            yield return null;
        }
    }
    
    private void ApplyFall()
    {
        if (!isGrounded)
        {
            _rb.AddForce(Vector3.up * (-1 * fallFactor), ForceMode.Acceleration);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        if (isGrounded)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_raycastHitPoint, 0.1f);
        }
    }
    
}
