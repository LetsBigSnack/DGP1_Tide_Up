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
        
        if (_onInteract != null && GameStateManager.Instance.GetGameState() == GameStates.MiniGame)
        {
            _onInteract.Invoke();
        }
    }
    
    private void PerformMoveLeft(InputAction.CallbackContext value)
    {
        
        if (_onMoveLeft != null && GameStateManager.Instance.GetGameState() == GameStates.MiniGame)
        {
            _onMoveLeft.Invoke(true);
        }
    }
    
    private void CancelMoveLeft(InputAction.CallbackContext value)
    {
        
        if (_onMoveLeft != null && GameStateManager.Instance.GetGameState() == GameStates.MiniGame)
        {
            _onMoveLeft.Invoke(false);
        }
    }

    private void PerformMoveRight(InputAction.CallbackContext value)
    {
        
        if (_onMoveRight != null && GameStateManager.Instance.GetGameState() == GameStates.MiniGame)
        {
            _onMoveRight.Invoke(true);
        }
    }
    
    private void CancelMoveRight(InputAction.CallbackContext value)
    {
        
        if (_onMoveRight != null && GameStateManager.Instance.GetGameState() == GameStates.MiniGame)
        {
            _onMoveRight.Invoke(false);
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
