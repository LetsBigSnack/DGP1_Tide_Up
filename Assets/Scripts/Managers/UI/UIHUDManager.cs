using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum KeyType
{
    Keyboard,
    Xbox,
    Playstation,
    Nintendo
}

//TODO export classes after review
[Serializable]
public class ToolbarVisualEntry
{
    [SerializeField]
    private KeyType type;
    [SerializeField]
    private string label;
    [SerializeField]
    private string key;
    [SerializeField]
    private Sprite iconBackground;

    public KeyType Type { get { return type; } set { type = value; } }
    public Sprite Sprite { get { return iconBackground; } set { iconBackground = value; } }
    public string Label { get { return label; } set { label = value; } }
    public string Key { get { return key; } set { key = value; } }
}

[Serializable]
public class ToolBarStateEntry
{
    [SerializeField]
    private GameStates stateType;

    [SerializeField]
    private KeyType keyType; 

    [SerializeField]
    private List<ToolbarVisualEntry> buttons;

    public GameStates StateType { get { return stateType; } set { stateType = value; } }
    public KeyType KeyType { get { return keyType; } set { keyType = value; } }
    public List<ToolbarVisualEntry> Buttons { get { return buttons; } set { buttons = value; } }
}

public class UIHUDManager : MonoBehaviour
{
    public static UIHUDManager Instance;

    [Header("currentKeyState")]
    [SerializeField]
    private GameStates currentKeyState;

    private KeyType currentKeyType;

    [Header("Toolbar")]
    [SerializeField] private GameObject toolBarItemPrefab;
    [SerializeField] private Transform toolBarContainer;
    [SerializeField] private bool toggleToolbar = true;
    [SerializeField] private List<ToolBarStateEntry> toolBarStateEntries;

    [Header("DateMap")]
    [SerializeField] private Transform dateMapContainer;


    [Header("CurrentButtons")]
    [SerializeField] private List<GameObject> currentButtons;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameStateManager.OnStateChanged += UpdateToolBar;
        UIPauseMenuManager.OnTooltipToggleChange += ToggleToolbar;

        //testing purpose for now
        currentKeyState = GameStates.PlayingBoat;
        UpdateToolBar(GameStates.PlayingCharacter);
    }

    private void OnDisable()
    {
        GameStateManager.OnStateChanged -= UpdateToolBar;
    }

    public void UpdateToolBar(GameStates state)
    {
        //currently hardcoded will need to be changed based on the controlles attached to the computer or currently active. with some kind of helper class
        //needs own ticket
        KeyType keyType = KeyType.Keyboard;

        if(currentKeyState == state && currentKeyType == keyType)
        {
            return;
        }

        if(currentButtons.Count > 0)
        {
            foreach(GameObject button in currentButtons)
            {
                Destroy(button);
            }
            currentButtons.Clear();
        }

        ToolBarStateEntry newEntry = toolBarStateEntries.Find(t => t.StateType == state && t.KeyType == keyType);

        if (newEntry != null)
        {
            foreach(ToolbarVisualEntry button in newEntry.Buttons)
            {
                GameObject newButton = Instantiate(toolBarItemPrefab, toolBarContainer);
                newButton.GetComponent<UIToolbarItem>().SetupButton(button.Sprite, button.Label, button.Key);
                currentButtons.Add(newButton);
            }
        }
        
        currentKeyState = state;
    }

    public void ToggleDateMap()
    {
        if(dateMapContainer != null)
        {
            bool isActive = dateMapContainer.gameObject.activeSelf;
            dateMapContainer.gameObject.SetActive(!isActive);
        }
    }

    public void ToggleToolbar(Toggle toggleState)
    {
        toggleToolbar = toggleState.isOn;
        if (toolBarContainer != null)
        {
            toolBarContainer.gameObject.SetActive(toggleToolbar);
        }
    }
}
