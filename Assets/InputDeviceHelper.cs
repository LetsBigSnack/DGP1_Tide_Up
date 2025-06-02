using System;
using UnityEngine;
using UnityEngine.InputSystem;


public enum DeviceType
{
    Keyboard,
    Mouse,
    Gamepad,
    Xbox,
    PlayStation,
    Unknown
}
public class InputDeviceHelper : MonoBehaviour
{
    public static InputDeviceHelper Instance { get; private set; }
    [SerializeField] DeviceType _lastDevice = DeviceType.Keyboard;
    [SerializeField] private DateTime _lastUseTime = DateTime.MinValue;

    public static Action<DeviceType> OnDeviceChange;
    
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool IsController()
    {
        return _lastDevice != DeviceType.Mouse && _lastDevice != DeviceType.Keyboard;
    }

    public DeviceType GetLastDeviceType()
    {
        return _lastDevice;
    }

    private float analogNoiseThreshold = 0.5f;
    public void NotifyDevice(InputControl control)
    {
        if (control == null || control.device == null)
            return;

        // Skip small analog input noise
        if (control.device is Gamepad)
        {
            float value = control.ReadValueAsObject() switch
            {
                float f => Mathf.Abs(f),
                Vector2 v => v.magnitude,
                _ => 0f
            };

            if (value < analogNoiseThreshold)
                return;
        }

        NotifyDevice(control.device); // Delegate to the original
    }

    public void NotifyDevice(InputDevice device)
    {
        DeviceType type = GetDeviceType(device);
        _lastUseTime = DateTime.UtcNow;
        if (type != _lastDevice)
        {
            _lastDevice = type;
            OnDeviceChange?.Invoke(type);
        }
    }
    
    private DeviceType GetDeviceType(InputDevice device)
    {
        if (device == null)
            return DeviceType.Unknown;
        if (device is Keyboard)
            return DeviceType.Keyboard;
        if (device is Mouse)
            return DeviceType.Mouse;
        if (device is Gamepad gp)
        {
            string lower = gp.displayName.ToLower();
            if (lower.Contains("xbox"))
                return DeviceType.Xbox;
            if (lower.Contains("wireless controller") || lower.Contains("dualshock") || lower.Contains("ps4"))
                return DeviceType.PlayStation;
            return DeviceType.Gamepad;
        }
        
        return DeviceType.Unknown;
    }
}
