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

    }

    private void MouseMoveBar(InputAction.CallbackContext value)
    {
        Vector2 axis = value.ReadValue<Vector2>();
        OnMoveBar?.Invoke(axis.x);
    }


    private void OnDisable()
    {
        _miniGameInput.Disable();
        
        _miniGameInput.MiniGame.Interact.Disable();
        _miniGameInput.MiniGame.Interact.performed -= Interact;
        
        _miniGameInput.MiniGame.MoveBar.Disable();
        _miniGameInput.MiniGame.MoveBar.performed -= MouseMoveBar;
    }
    
    private void Interact(InputAction.CallbackContext value)
    {
        
        if (_onInteract != null && GameStateManager.Instance.GetGameState() == GameStates.MiniGame)
        {
            _onInteract.Invoke();
        }
    }
  
    
    public void RegisterMiniGameInteract(Action callback)
    {
        _onInteract += callback;
    }

    public void UnregisterMiniGameInteract(Action callback)
    {
        _onInteract -= callback;
    }


}
