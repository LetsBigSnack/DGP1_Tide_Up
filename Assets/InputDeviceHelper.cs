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
        Debug.Log(device + " " + control);

        if (device is Gamepad)
        {
            // 2. Filter axis inputs (e.g., triggers)
            if (control is AxisControl axis)
            {
                float value = Mathf.Abs(axis.ReadValue());
                Debug.Log($"[Input] AxisControl '{control.name}' = {value}");

                if (value < analogNoiseThreshold)
                    return;
            }

            if (control is Vector2Control vector2)
            {
                float mag = vector2.ReadValue().magnitude;
                Debug.Log($"[Input] Vector2Control '{control.name}' = {mag}");

                if (mag < analogNoiseThreshold)
                    return;
            }
        }

        DeviceType type = GetDeviceType(device);
        _lastUseTime = DateTime.UtcNow;
        if (type != _lastDevice)
        {
            Debug.Log(device);
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
