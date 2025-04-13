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
        _menuInputs.UI.CloseMenu.performed += CloseMenu;
    }
    
    private void OnDisable()
    {
        _menuInputs.UI.Disable();
        
        //Inventory
        _menuInputs.UI.Inventory.Disable();
        _menuInputs.UI.Inventory.performed -= ShowInventory;
        _menuInputs.UI.CloseMenu.performed -= CloseMenu;
    }

    private void ShowInventory(InputAction.CallbackContext value)
    {
        GameStateManager.Instance.SetGameState(GameStates.InMenu);
        UIHUDManager.Instance.ToggleDateMap();
        UIJournalManager.Instance.SwitchState(JournalType.Inventory);
    }
    private void CloseMenu(InputAction.CallbackContext value)
    {
        GameStateManager.Instance.SetGameState(GameStates.PlayingCharacter);
        UIHUDManager.Instance.ToggleDateMap();
        UIJournalManager.Instance.CloseAllMenues();
    }

}
