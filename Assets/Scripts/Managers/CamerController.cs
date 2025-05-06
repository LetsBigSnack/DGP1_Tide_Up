using System;
using System.Collections.Generic;
using UnityEngine;


public enum CameraTarget
{
    Player,
    Boat
}



[Serializable]
public class CameraPreset
{
    [SerializeField] private CameraTarget cameraTarget;

    public CameraTarget CameraTarget
    {
        get => cameraTarget;
        set => cameraTarget = value;
    }

    public float DistanceToPlayer
    {
        get => distanceToPlayer;
        set => distanceToPlayer = value;
    }

    public Vector3 Offset
    {
        get => offset;
        set => offset = value;
    }

    public float Yaw
    {
        get => yaw;
        set => yaw = value;
    }

    public float Pitch
    {
        get => pitch;
        set => pitch = value;
    }

    public float Roll
    {
        get => roll;
        set => roll = value;
    }

    public float SmoothTime
    {
        get => smoothTime;
        set => smoothTime = value;
    }

    [SerializeField] private float distanceToPlayer;
    [SerializeField] private Vector3 offset;
    [Range(0f, 360f)] public float yaw = 0f;  
    [Range(-90f, 90f)] public float pitch = 0f; 
    [Range(-180f, 180f)] public float roll = 0f; 
    [SerializeField] private float smoothTime;
}

public class CamerController : MonoBehaviour
{
    public static CamerController Instance;
    
    
    private Transform _target;
    
    [Header("Camera Settings")]
    [SerializeField] private CameraTarget cameraTarget = CameraTarget.Player;
    [SerializeField] private List<CameraPreset> presets;
    
    
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
            CameraPreset preset = presets.Find(c => c.CameraTarget == cameraTarget);
            
            
            Vector3 normalizedOffset = Vector3.Normalize(preset.Offset);
            Vector3 targetPosition = (_target.position + normalizedOffset * preset.DistanceToPlayer);
            transform.position =  Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, preset.SmoothTime);
        
            Quaternion baseLookRotation = Quaternion.LookRotation(normalizedOffset * preset.DistanceToPlayer);
            
            Quaternion parametrizedRotation = baseLookRotation * Quaternion.Euler(preset.Pitch, preset.Yaw, preset.Roll);
            
            transform.rotation = parametrizedRotation;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
