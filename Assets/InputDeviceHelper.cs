using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

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
    [SerializeField] private float analogNoiseThreshold = 0.5f;

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

    public void NotifyDevice(InputDevice device, InputControl control)
    {
        if (device is Gamepad)
        {
            if (control is AxisControl axis && Mathf.Abs(axis.ReadValue()) < analogNoiseThreshold)
                return;

            if (control is Vector2Control vector2 && vector2.ReadValue().magnitude < analogNoiseThreshold)
                return;

            if (control is ButtonControl button)
                if (!button.isPressed)
                    return;
        }

        DeviceType type = GetDeviceType(device);
        if (type != _lastDevice)
        {
            _lastUseTime = DateTime.UtcNow;
            _lastDevice = type;
            OnDeviceChange?.Invoke(type);
#if UNITY_EDITOR
            //Debug.Log($"Switched to {type} via {control.name}");
#endif
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
