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
    private DeviceType keyType; 

    [SerializeField]
    private List<ButtonInputType> buttonTypes;

    public GameStates StateType { get { return stateType; } set { stateType = value; } }
    public DeviceType KeyType { get { return keyType; } set { keyType = value; } }
    public List<ButtonInputType> ButtonInputTypes { get { return buttonTypes; } set { buttonTypes = value; } }
}

public class UIHUDManager : MonoBehaviour
{
    public static UIHUDManager Instance;

    [Header("currentStates")]
    [SerializeField]
    private GameStates currentState;
    [SerializeField]
    private DeviceType currentKeyType;

    [Header("GameState_Toolbars")]
    [SerializeField] private List<ToolBarStateEntry> entries;

    [Header("Tutorial_Toolbar")]
    [SerializeField] private List<ToolBarStateEntry> tutorialEntry;
    [Header("TideUpBox_Toolbar")]
    [SerializeField] private ToolBarStateEntry tideUpBoxEntry;
    [Header("ReUpCycler_Toolbar")]
    [SerializeField] private ToolBarStateEntry reUpCyclerBoxEntry;
    [Header("Journal_Toolbar")]
    [SerializeField] private ToolBarStateEntry journalEntry;

    [Header("Prefabs")]
    [SerializeField] private GameObject toolBarItemPrefab;
    [SerializeField] private Transform toolBarContainer;
    [SerializeField] private bool toggleToolbar = true;

    [Header("DateMap")]
    [SerializeField] private Transform dateMapContainer;
    [SerializeField] private Image dayNightImg;
    [SerializeField] private Image seasonImg;

    [Header("Seasons")]
    [SerializeField] private Sprite daySprite;
    [SerializeField] private Sprite nightSprite;

    [Header("Seasons")]
    [SerializeField] private Sprite winterSprite;
    [SerializeField] private Sprite springSprite;
    [SerializeField] private Sprite summerSprite;
    [SerializeField] private Sprite autumnSprite;
    private Seasons _currSeason;

    [Header("CurrentButtons")]
    [SerializeField] private List<GameObject> currentButtons;

    [Header("ButtonSO's")]
    [SerializeField] private List<ToolBarSetting> buttonsEntry;

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
        InputDeviceHelper.OnDeviceChange += UpdateToolBarByDevice;
        UIPauseMenuManager.OnTooltipToggleChange += ToggleToolbar;
        TimeManager.OnTimeChanged += UpdateDayNight;
        TimeManager.OnMonthChanged += UpdateSeason;
    }

    private void OnDisable()
    {
        GameStateManager.OnStateChanged -= UpdateToolBar;
        InputDeviceHelper.OnDeviceChange -= UpdateToolBarByDevice;
    }

    public void UpdateToolBarByDevice(DeviceType type)
    {
        UpdateToolBar(GameStateManager.Instance.GetGameState());
    }

    public void UpdateToolBar(GameStates state)
    {
        DeviceType keyType = InputDeviceHelper.Instance.GetLastDeviceType();



        if (currentState == state && currentKeyType == keyType)
        {
            return;
        }
        ClearButtons();
        if(InputDeviceHelper.Instance.GetLastDeviceType() == DeviceType.Mouse)
        {
            currentKeyType = DeviceType.Keyboard;
        }
        else
        {
            currentKeyType = InputDeviceHelper.Instance.GetLastDeviceType();
        }
        
        currentState = state;

        if (TutorialManager.Instance != null)
        {
            CreateToolBarButtonsFromList(state, keyType, tutorialEntry);
            return;
        }

        if (UITideUpBoxManager.Instance.IsOpen && state != GameStates.PlayingCharacter)
        {
            CreateToolBarButtonsFromEntry(keyType, tideUpBoxEntry);
            return;
        }

        if(UIReUpcycleManager.Instance.GetCurrentState() != ReUpcyclerType.Closed && state != GameStates.PlayingCharacter)
        {
            CreateToolBarButtonsFromEntry(keyType, reUpCyclerBoxEntry);
            return;
        }

        if(UIJournalManager.Instance.GetCurrentState() != JournalType.Closed && state != GameStates.PlayingCharacter)
        {
            CreateToolBarButtonsFromEntry(keyType, journalEntry);
            return;
        }

        CreateToolBarButtonsFromList(state, keyType, entries);
    }

    private void ClearButtons()
    {
        if (currentButtons.Count > 0)
        {
            foreach (GameObject button in currentButtons)
            {
                Destroy(button);
            }
            currentButtons.Clear();
        }
    }

    private void CreateToolBarButtonsFromList(GameStates state, DeviceType keyType, List<ToolBarStateEntry> list)
    {
        if(keyType == DeviceType.Mouse)
        {
            return;
        }

        List<ButtonInputType> buttons = list.Find(t => t.StateType == state)?.ButtonInputTypes;
        if (buttons != null)
        {
            ToolBarSetting curSetting = buttonsEntry.Find(b => b.Type == keyType);
            foreach (ButtonInputType type in buttons)
            {
                ButtonEntry newButtonEntry = curSetting?.ButtonEntries?.Find(buttonEntry => buttonEntry.Type == type);
                if (!newButtonEntry.NotNeeded)
                {
                    GameObject newButton = Instantiate(toolBarItemPrefab, toolBarContainer);
                    newButton.GetComponent<UIToolbarItem>().SetupButton(newButtonEntry.Sprite ?? null, newButtonEntry.Type.ToString(), newButtonEntry.Key, keyType);
                    currentButtons.Add(newButton);
                }
            }
        }
    }

    private void CreateToolBarButtonsFromEntry(DeviceType keyType, ToolBarStateEntry entry)
    {
        if (keyType == DeviceType.Mouse)
        {
            return;
        }

        ToolBarSetting curSetting = buttonsEntry.Find(b => b.Type == keyType);
        foreach (ButtonInputType type in entry.ButtonInputTypes)
        {
            ButtonEntry newButtonEntry = curSetting.ButtonEntries.Find(buttonEntry => buttonEntry.Type == type);
            if (!newButtonEntry.NotNeeded)
            {
                GameObject newButton = Instantiate(toolBarItemPrefab, toolBarContainer);
                newButton.GetComponent<UIToolbarItem>().SetupButton(newButtonEntry.Sprite ?? null, newButtonEntry.Type.ToString(), newButtonEntry.Key, keyType);
                currentButtons.Add(newButton);
            }
        }
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

    private void UpdateDayNight(float currTime)
    {
        if (daySprite == null || nightSprite == null)
        {
            return;
        }
        
        if(currTime >= 6f && currTime <= 18f)
        {
            dayNightImg.sprite = daySprite;
            return;
        }
        dayNightImg.sprite = nightSprite;

    }
    private void UpdateSeason(int newSeason)
    {
        if(newSeason == (int)_currSeason)
        {
            return;
        }

        switch ((Seasons)newSeason)
        {
            case Seasons.Winter:
                _currSeason = Seasons.Winter;
                seasonImg.sprite = winterSprite;
                break;
            case Seasons.Spring:
                _currSeason = Seasons.Spring;
                seasonImg.sprite = springSprite;
                break;
            case Seasons.Summer:
                _currSeason = Seasons.Summer;
                seasonImg.sprite = summerSprite;
                break;
            case Seasons.Autumn:
                _currSeason = Seasons.Autumn;
                seasonImg.sprite = autumnSprite;
                break;
        }

    }
}
