using System.Collections.Generic;
using Data;
using Unity.VisualScripting;
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
        
        _menuInputs.UI.Inventory.Enable();
        _menuInputs.UI.Inventory.performed += ShowInventory;
        
        _menuInputs.UI.CloseMenu.Enable();
        _menuInputs.UI.CloseMenu.performed += HandleEscape;

        _menuInputs.UI.ToggleX.Enable();
        _menuInputs.UI.ToggleX.performed += ToggleMenuItem;

        _menuInputs.UI.Friends.Enable();
        _menuInputs.UI.Friends.performed += ShowFriends;

        _menuInputs.UI.Quests.Enable();
        _menuInputs.UI.Quests.performed += ShowQuests;

        _menuInputs.UI.Calender.Enable();
        _menuInputs.UI.Calender.performed += ShowCalender;

        _menuInputs.UI.Recipe.Enable();
        _menuInputs.UI.Recipe.performed += ShowRecipes;

        _menuInputs.UI.Map.Enable();
        _menuInputs.UI.Map.performed += ShowMap;

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

        _menuInputs.UI.Map.Disable();
        _menuInputs.UI.Map.performed -= ShowMap;

        _menuInputs.UI.Friends.Disable();
        _menuInputs.UI.Friends.performed -= ShowFriends;

        _menuInputs.UI.Quests.Disable();
        _menuInputs.UI.Quests.performed -= ShowQuests;

        _menuInputs.UI.Calender.Disable();
        _menuInputs.UI.Calender.performed -= ShowCalender;

        _menuInputs.UI.Recipe.Disable();
        _menuInputs.UI.Recipe.performed -= ShowRecipes;

    }

    private bool CanMenuBeOpen()
    {
        List<GameStates> notAllowed = new List<GameStates>();
        notAllowed.Add(GameStates.Dialogue);
        notAllowed.Add(GameStates.SceneTransition);
        notAllowed.Add(GameStates.Paused);
        notAllowed.Add(GameStates.MiniGame);
        notAllowed.Add(GameStates.Building);
        
        return !notAllowed.Contains(GameStateManager.Instance.GetGameState()) && TutorialManager.Instance == null && UIShopManager.Instance.GetCurrentState() == ShopType.Closed && UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Closed;
    }

    private bool IsSameMenuOpen(JournalType journalType)
    {
        return UIJournalManager.Instance.GetCurrentState() == journalType;
    }

    private void ShowInventory(InputAction.CallbackContext value)
    {
        if (!CanMenuBeOpen())
        {
            return;
        }

        if (IsSameMenuOpen(JournalType.Inventory))
        {
            CloseMenu();
            return;
        }
        
        GameStateManager.Instance.SetGameState(GameStates.InMenu);
        UIJournalManager.Instance.SwitchState(JournalType.Inventory);
    }

    private void ShowMap(InputAction.CallbackContext value)
    {
        if (!CanMenuBeOpen())
        {
            return;
        }
        
        if (IsSameMenuOpen(JournalType.Map))
        {
            CloseMenu();
            return;
        }
        
        GameStateManager.Instance.SetGameState(GameStates.InMenu);
        UIJournalManager.Instance.SwitchState(JournalType.Map);
    }

    private void ShowRecipes(InputAction.CallbackContext value)
    {
        if (!CanMenuBeOpen())
        {
            return;
        }
        
        if (IsSameMenuOpen(JournalType.Recipies))
        {
            CloseMenu();
            return;
        }
        
        GameStateManager.Instance.SetGameState(GameStates.InMenu);
        UIJournalManager.Instance.SwitchState(JournalType.Recipies);
    }

    private void ShowFriends(InputAction.CallbackContext value)
    {
        if (!CanMenuBeOpen())
        {
            return;
        }
        
        if (IsSameMenuOpen(JournalType.FriendBook))
        {
            CloseMenu();
            return;
        }
        
        GameStateManager.Instance.SetGameState(GameStates.InMenu);
        UIJournalManager.Instance.SwitchState(JournalType.FriendBook);
    }

    private void ShowQuests(InputAction.CallbackContext value)
    {
        if (!CanMenuBeOpen())
        {
            return;
        }
        
        if (IsSameMenuOpen(JournalType.Quests))
        {
            CloseMenu();
            return;
        }
        
        GameStateManager.Instance.SetGameState(GameStates.InMenu);
        UIJournalManager.Instance.SwitchState(JournalType.Quests);
    }

    private void ShowCalender(InputAction.CallbackContext value)
    {
        if (!CanMenuBeOpen())
        {
            return;
        }
        
        if (IsSameMenuOpen(JournalType.Calender))
        {
            CloseMenu();
            return;
        }
        
        GameStateManager.Instance.SetGameState(GameStates.InMenu);
        UIJournalManager.Instance.SwitchState(JournalType.Calender);
    }
    private void CloseMenu()
    {
        GameStateManager.Instance.SetGameState(GameStateManager.Instance.LastPlayingState);
        //TODO: Grenus Fix
        //UIHUDManager.Instance.ToggleDateMap();
        UIBuildManager.Instance.CloseMenu();
        UIJournalManager.Instance.SwitchState(JournalType.Closed);
        UIReUpcycleManager.Instance.SwitchState(ReUpcyclerType.Closed);
        UIShopManager.Instance.SwitchState(ShopType.Closed);
        UITideUpBoxManager.Instance.CloseTideUpBox();
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


    private void HandleEscape(InputAction.CallbackContext value)
    {
        var currentState = GameStateManager.Instance.GetGameState();

        if (currentState == GameStates.PlayingCharacter || currentState == GameStates.Paused || currentState == GameStates.PlayingBoat)
        {
            if(UIPauseMenuManager.Instance.PauseMenuState == PauseMenuStates.Off || UIPauseMenuManager.Instance.PauseMenuState == PauseMenuStates.Paused) 
            {
                UIPauseMenuManager.Instance.TogglePauseGame();
            }
            else if(UIPauseMenuManager.Instance.PauseMenuState == PauseMenuStates.Options)
            {
                UIPauseMenuManager.Instance.ToggleOptionMenu();
            }
        }
        else if (currentState == GameStates.InMenu || currentState == GameStates.Building)
        {
            CloseMenu(); // optionally pass null instead
        }
        else if (currentState == GameStates.Dialogue && TutorialManager.Instance == null)
        {
            NpcDialogueManager.Instance.ResetDialogue();
            UIDialogueManager.Instance.FinishSpeaking();
        }
    }
}
