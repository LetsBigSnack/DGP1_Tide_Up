using Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    
    private MenuInputs _menuInputs;
    

    private void Awake()
    {
        _menuInputs = new MenuInputs();
    }
    
    private void OnEnable()
    {
        _menuInputs.Enable();
        
        //Inventory
        _menuInputs.UI.Inventory.Enable();
        _menuInputs.UI.Inventory.performed += ShowInventory;
        
        _menuInputs.UI.CloseMenu.Enable();
        _menuInputs.UI.CloseMenu.performed += HandleEscape;
        
    }
    
    private void OnDisable()
    {
        _menuInputs.UI.Disable();

   
        //Inventory
        _menuInputs.UI.Inventory.Disable();
        _menuInputs.UI.Inventory.performed -= ShowInventory;
        
        _menuInputs.UI.CloseMenu.Disable();
        _menuInputs.UI.CloseMenu.performed -= HandleEscape;
        

    }

    private void ShowInventory(InputAction.CallbackContext value)
    {
        if (GameStateManager.Instance.GetGameState() == GameStates.Dialogue || GameStateManager.Instance.GetGameState() == GameStates.InMenu || GameStateManager.Instance.GetGameState() == GameStates.SceneTransition)
        {
            return;
        }
        GameStateManager.Instance.SetGameState(GameStates.InMenu);
        //TODO: Grenus Fix
        //UIHUDManager.Instance.ToggleDateMap();
        UIJournalManager.Instance.SwitchState(JournalType.Inventory);
    }
    private void CloseMenu()
    {
        GameStateManager.Instance.SetGameState(GameStates.PlayingCharacter);
        //TODO: Grenus Fix
        //UIHUDManager.Instance.ToggleDateMap();
        UIJournalManager.Instance.CloseAllMenues();
        UIReUpcycleManager.Instance.CloseAllMenues();
        UIShopManager.Instance.CloseAllMenues();
        //TODO: This is completely breaking the time manager deki 24.04.2025
        //UITimeManager.Instance.CancelTimeChange();
    }

    private void HandleEscape(InputAction.CallbackContext value)
    {
        var currentState = GameStateManager.Instance.GetGameState();

        if (currentState == GameStates.PlayingCharacter || currentState == GameStates.Paused)
        {
            UIPauseMenuManager.Instance.TogglePauseGame();
        }
        else if (currentState == GameStates.InMenu)
        {
            CloseMenu(); // optionally pass null instead
        }
        else if (currentState == GameStates.Dialogue)
        {
            NpcDialogueManager.Instance.ResetDialogue();
        }
    }
}
