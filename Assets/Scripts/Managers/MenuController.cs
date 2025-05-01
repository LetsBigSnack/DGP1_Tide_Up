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

        _menuInputs.UI.ToggleX.Enable();
        _menuInputs.UI.ToggleX.performed += ToggleMenuItem;

        _menuInputs.UI.Map.Enable();
        _menuInputs.UI.Map.performed += OpenMapMenu;

    }
    
    private void OnDisable()
    {
        _menuInputs.UI.Disable();

   
        //Inventory
        _menuInputs.UI.Inventory.Disable();
        _menuInputs.UI.Inventory.performed -= ShowInventory;
        
        _menuInputs.UI.CloseMenu.Disable();
        _menuInputs.UI.CloseMenu.performed -= HandleEscape;

        _menuInputs.UI.ToggleX.Disable();
        _menuInputs.UI.ToggleX.performed -= ToggleMenuItem;

        _menuInputs.UI.Map.Enable();
        _menuInputs.UI.Map.performed -= OpenMapMenu;

    }

    private void ShowInventory(InputAction.CallbackContext value)
    {
        if (GameStateManager.Instance.GetGameState() == GameStates.Dialogue || GameStateManager.Instance.GetGameState() == GameStates.SceneTransition)
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
        UIJournalManager.Instance.SwitchState(JournalType.Closed);
        UIReUpcycleManager.Instance.SwitchState(ReUpcyclerType.Closed);
        UIShopManager.Instance.SwitchState(ShopType.Closed);
        if (UITimeManager.Instance.IsTimeChangeActive())
        {
            UITimeManager.Instance.CancelTimeChange();
        }
    }
    
    private void ToggleMenuItem(InputAction.CallbackContext value)
    {
        if (GameStateManager.Instance.GetGameState() != GameStates.InMenu)
        {
            return;
        }

        if(UIJournalManager.Instance.GetCurrentState() == JournalType.Map)
        {
            UIMapController.Instance.ToggleVisiblePlaces();
        }
    }

    private void OpenMapMenu(InputAction.CallbackContext value)
    {
        GameStateManager.Instance.SetGameState(GameStates.InMenu);
        UIJournalManager.Instance.SwitchState(JournalType.Map);
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
