using Helpers.Util;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    private PlayerInputs _playerInputs;
    private Rigidbody _rb;

    [Header("Boat Settings")]
    public float acceleration = 10f;
    public float maxSpeed = 8f;
    public float turnSpeed = 50f;
    public float waterDrag = 2f;
    public float driftFactor = 0.95f; // 1 = no drift, lower = more drift

    private float _steerInput = 0f;
    private float _throttleInput = 0f;

    private void Awake()
    {
        _playerInputs = new PlayerInputs();
        _rb = GetComponent<Rigidbody>();

        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void OnEnable()
    {
        _playerInputs.Enable();
        _playerInputs.Boat.Movement.Enable();
        _playerInputs.Boat.Movement.performed += OnMoveBoatPerformed;
        _playerInputs.Boat.Movement.canceled += OnMoveBoatCancelled;
    }

    private void OnDisable()
    {
        _playerInputs.Boat.Movement.Disable();
        _playerInputs.Disable();
        _playerInputs.Boat.Movement.performed -= OnMoveBoatPerformed;
        _playerInputs.Boat.Movement.canceled -= OnMoveBoatCancelled;
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

        MoveBoat();
    }

    private void MoveBoat()
    {
        // Limit velocity
        Vector3 flatVel = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        if (flatVel.magnitude > maxSpeed)
        {
            flatVel = flatVel.normalized * maxSpeed;
            _rb.linearVelocity = new Vector3(flatVel.x, _rb.linearVelocity.y, flatVel.z);
        }

        // Apply Forward Force
        Vector3 force = transform.forward * (_throttleInput * acceleration);
        _rb.AddForce(force, ForceMode.Acceleration);

        // Apply Steering
        if (flatVel.magnitude > 0.1f) // Only steer if moving
        {
            float turnAmount = _steerInput * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnOffset = Quaternion.Euler(0f, turnAmount, 0f);
            _rb.MoveRotation(_rb.rotation * turnOffset);
        }

        // Apply simple drift (reduce side velocity)
        Vector3 localVel = transform.InverseTransformDirection(_rb.linearVelocity);
        localVel.x *= driftFactor; // Dampen sideways speed
        _rb.linearVelocity = transform.TransformDirection(localVel);

        // Water Drag
        _rb.linearDamping = waterDrag;
    }
}
