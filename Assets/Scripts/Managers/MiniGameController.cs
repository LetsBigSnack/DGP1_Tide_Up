using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;


public class MiniGameController : MonoBehaviour
{
    
    public static MiniGameController Instance;


    private MiniGameInputs _miniGameInput;

    private Action _onInteract;
    private Action<bool> _onMoveLeft;
    private Action<bool> _onMoveRight;
    
    private Action<bool> _onButton1;
    private Action<bool> _onButton2;
    private Action<bool> _onButton3;
    private Action<bool> _onButton4;
    
    public static event Action<float> OnMoveBar;

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _miniGameInput = new MiniGameInputs();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    
    private void OnEnable()
    {
        _miniGameInput.Enable();
        
        //Interact
        _miniGameInput.MiniGame.Interact.Enable();
        _miniGameInput.MiniGame.Interact.performed += Interact;
        
        _miniGameInput.MiniGame.MoveBar.Enable();
        _miniGameInput.MiniGame.MoveBar.performed += MouseMoveBar;
        
        _miniGameInput.MiniGame.MoveLeft.Enable();
        _miniGameInput.MiniGame.MoveLeft.performed += PerformMoveLeft;
        _miniGameInput.MiniGame.MoveLeft.canceled += CancelMoveLeft;
        
        _miniGameInput.MiniGame.MoveRight.Enable();
        _miniGameInput.MiniGame.MoveRight.performed += PerformMoveRight;
        _miniGameInput.MiniGame.MoveRight.canceled += CancelMoveRight;
        
        
        _miniGameInput.MiniGame.Button1.Enable();
        _miniGameInput.MiniGame.Button2.Enable();
        _miniGameInput.MiniGame.Button3.Enable();
        _miniGameInput.MiniGame.Button4.Enable();

        _miniGameInput.MiniGame.Button1.performed += OnButton1Performe;
        _miniGameInput.MiniGame.Button2.performed += OnButton2Performe;
        _miniGameInput.MiniGame.Button3.performed += OnButton3Performe;
        _miniGameInput.MiniGame.Button4.performed += OnButton4Performe;
    }

    private void MouseMoveBar(InputAction.CallbackContext value)
    {
        Vector2 axis = value.ReadValue<Vector2>();
        OnMoveBar?.Invoke(axis.x);
    }


    private void OnDisable()
    {
        if(_miniGameInput == null)
        {
            return;
        }

        _miniGameInput.Disable();
        
        _miniGameInput.MiniGame.Interact.Disable();
        _miniGameInput.MiniGame.Interact.performed -= Interact;
        
        _miniGameInput.MiniGame.MoveBar.Disable();
        _miniGameInput.MiniGame.MoveBar.performed -= MouseMoveBar;
        
        _miniGameInput.MiniGame.MoveLeft.Disable();
        _miniGameInput.MiniGame.MoveLeft.performed -= PerformMoveLeft;
        _miniGameInput.MiniGame.MoveLeft.canceled -= CancelMoveLeft;
        
        _miniGameInput.MiniGame.MoveRight.Disable();
        _miniGameInput.MiniGame.MoveRight.performed -= PerformMoveRight;
        _miniGameInput.MiniGame.MoveRight.canceled -= CancelMoveRight;
    }
    
    private void Interact(InputAction.CallbackContext value)
    {
        InputDeviceHelper.Instance?.NotifyDevice(value.control.device, value.control);
        if (_onInteract != null && GameStateManager.Instance.GetGameState() == GameStates.MiniGame)
        {
            _onInteract.Invoke();
        }
    }
    
    private void PerformMoveLeft(InputAction.CallbackContext value)
    {
        InputDeviceHelper.Instance?.NotifyDevice(value.control.device, value.control);
        if (_onMoveLeft != null && GameStateManager.Instance.GetGameState() == GameStates.MiniGame)
        {
            _onMoveLeft.Invoke(true);
        }
    }
    
    private void CancelMoveLeft(InputAction.CallbackContext value)
    {
        InputDeviceHelper.Instance?.NotifyDevice(value.control.device, value.control);
        if (_onMoveLeft != null && GameStateManager.Instance.GetGameState() == GameStates.MiniGame)
        {
            _onMoveLeft.Invoke(false);
        }
    }

    private void PerformMoveRight(InputAction.CallbackContext value)
    {
        InputDeviceHelper.Instance?.NotifyDevice(value.control.device, value.control);
        if (_onMoveRight != null && GameStateManager.Instance.GetGameState() == GameStates.MiniGame)
        {
            _onMoveRight.Invoke(true);
        }
    }
    
    private void CancelMoveRight(InputAction.CallbackContext value)
    {
        InputDeviceHelper.Instance?.NotifyDevice(value.control.device, value.control);
        if (_onMoveRight != null && GameStateManager.Instance.GetGameState() == GameStates.MiniGame)
        {
            _onMoveRight.Invoke(false);
        }
    }
    
    
    private void OnButton1Performe(InputAction.CallbackContext value)
    {
        InputDeviceHelper.Instance?.NotifyDevice(value.control.device, value.control);
        if (_onButton1 != null && GameStateManager.Instance.GetGameState() == GameStates.MiniGame)
        {
            _onButton1.Invoke(true);
        }
    }
    
    private void OnButton2Performe(InputAction.CallbackContext value)
    {
        InputDeviceHelper.Instance?.NotifyDevice(value.control.device, value.control);
        if (_onButton2 != null && GameStateManager.Instance.GetGameState() == GameStates.MiniGame)
        {
            _onButton2.Invoke(true);
        }
    }
    
    private void OnButton3Performe(InputAction.CallbackContext value)
    {
        InputDeviceHelper.Instance?.NotifyDevice(value.control.device, value.control);
        if (_onButton3 != null && GameStateManager.Instance.GetGameState() == GameStates.MiniGame)
        {
            _onButton3.Invoke(true);
        }
    }
    
    private void OnButton4Performe(InputAction.CallbackContext value)
    {
        InputDeviceHelper.Instance?.NotifyDevice(value.control.device, value.control);
        if (_onButton4 != null && GameStateManager.Instance.GetGameState() == GameStates.MiniGame)
        {
            _onButton4.Invoke(true);
        }
    }
    
    
    
    
    public void RegisterMiniGameInteract(Action callback, Action<bool> onMoveLeft = null, Action<bool> onMoveRight = null)
    {
        _onInteract += callback;
        if (onMoveLeft != null)
        {
            _onMoveLeft += onMoveLeft;
        }
        if (onMoveRight != null)
        {
            _onMoveRight += onMoveRight;
        }
    }
    
    public void RegisterMiniGameBoat(Action<bool> button1, Action<bool> button2, Action<bool> button3, Action<bool> button4)
    {
        _onButton1 += button1;
        _onButton2 += button2;
        _onButton3 += button3;
        _onButton4 += button4;
    }
    
    public void UnregisterMiniGameBoat(Action<bool> button1, Action<bool> button2, Action<bool> button3, Action<bool> button4)
    {
        _onButton1 -= button1;
        _onButton2 -= button2;
        _onButton3 -= button3;
        _onButton4 -= button4;
    }

    public void UnregisterMiniGameInteract(Action callback,  Action<bool> onMoveLeft = null, Action<bool> onMoveRight = null)
    {
        _onInteract -= callback;
        if (onMoveLeft != null)
        {
            _onMoveLeft -= onMoveLeft;
        }
        if (onMoveRight != null)
        {
            _onMoveRight -= onMoveRight;
        }
    }


}
