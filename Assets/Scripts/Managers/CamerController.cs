using System;
using UnityEngine;


public enum CameraTarget
{
    Player,
    Boat
}


public class CamerController : MonoBehaviour
{
    public static CamerController Instance;
    
    
    private Transform _target;
    
    [Header("Camera Settings")]
    [SerializeField] private CameraTarget cameraTarget = CameraTarget.Player;
    [SerializeField] private float distanceToPlayer;
    [SerializeField] private Vector3 offset;
    [Range(0f, 360f)] public float yaw = 0f;  
    [Range(-90f, 90f)] public float pitch = 0f; 
    [Range(-180f, 180f)] public float roll = 0f; 
    [SerializeField] private float smoothTime;
    
    
    private Vector3 _velocity;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SwitchTarget(CameraTarget.Player);
    }


    public void SwitchTarget(CameraTarget newTarget)
    {
        
        cameraTarget = newTarget;
        
        switch (cameraTarget)
        {
            case CameraTarget.Player:
                _target = Player.Instance.gameObject.transform;
                break;
            case CameraTarget.Boat:
                _target = Boat.Instance.gameObject.transform;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    void LateUpdate()
    {
        try
        {
            Vector3 normalizedOffset = Vector3.Normalize(offset);
            Vector3 targetPosition = (_target.position + normalizedOffset * distanceToPlayer);
            transform.position =  Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);
        
            Quaternion baseLookRotation = Quaternion.LookRotation(normalizedOffset * distanceToPlayer);
            
            Quaternion parametrizedRotation = baseLookRotation * Quaternion.Euler(pitch, yaw, roll);
            
            transform.rotation = parametrizedRotation;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
