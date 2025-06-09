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

}
