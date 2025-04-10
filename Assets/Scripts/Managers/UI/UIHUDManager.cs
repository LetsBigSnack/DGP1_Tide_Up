using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//with this we can set the right image sprite as well for each button :)
public enum KeyType
{
    Keyboard,
    Xbox,
    Playstation,
    Nintendo
}

//you've already importet System no need to do it twice :D
//[System.Serializable]
[Serializable]
//I added the possibility to have a background image and a "proper" key -> in case we want to switch keys later(keybindings and stuff) on -> this is generally more future proof :)

//Maybe SO?? changing something in the manager could clear the list and we need to do all the things over again -> having a backup would be nice
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

//better approach since we can now dynamically change everything in the editor and are not bound to touch the script everytime something might change, especially since we might not know all the buttons for now <3
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
    //personally love the instance at top haha so it feels like its the right sequenze if it makes sense :D
    public static UIHUDManager Instance;

    [Header("currentKeyState")]
    [SerializeField]
    private GameStates currentKeyState;
    //based on what we want to display or change letter on depending on the controller we're using :D
    private KeyType currentKeyType = KeyType.Keyboard;

    [Header("Toolbar")]
    [SerializeField] private GameObject toolBarItemPrefab;
    [SerializeField] private Transform toolBarContainer;
    //setting a boolean true here is not really necessary since the default state is true but this is just nitpicking haha
    [SerializeField] private bool toggleToolbar = true;
    [SerializeField] private List<ToolBarStateEntry> toolBarStateEntries;

    [Header("DateMap")]
    [SerializeField] private Transform dateMapContainer;

    //Searching the transform is not the coolest way to do stuff, the better approach is to create a list that currently tracks our buttons and we can ezpz clean it up if necessary :3
    [Header("CurrentButtons")]
    [SerializeField] private List<GameObject> currentButtons;

    //generally a very good approach I like a lot! But let's polish it a bit :D
    //private Dictionary<GameStates, List<ToolbarVisualEntry>> _toolbarVisuals = new(); I dont know why but i hate the new(); thingy xD y did the decide to remove the rest feels so wrong hahahaha
    //Since we now know that our ToolBarStateEntry has a gamestate aswell we dont even need a dictionary <3

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameStateManager.OnStateChanged += UpdateToolBar;
        //No need for this anymore :3
        //ToolbarSetup();
    }

    private void OnDisable()
    {
        GameStateManager.OnStateChanged -= UpdateToolBar;
    }


    //We completely annihalited with this approach the need for this :D No magic names! yaaaaay


    /*
    private void ToolbarSetup()
    {
        _toolbarVisuals[GameStates.PlayingCharacter] = new List<ToolbarVisualEntry>
        {
            new ToolbarVisualEntry { label = "Inventory", icon = GetSpriteByName("test2") },
            new ToolbarVisualEntry { label = "Recipes", icon = GetSpriteByName("test") },
            new ToolbarVisualEntry { label = "Tasks", icon = GetSpriteByName("test2") },
            new ToolbarVisualEntry { label = "Friendbook", icon = GetSpriteByName("test") }
        };

        _toolbarVisuals[GameStates.PlayingBoat] = new List<ToolbarVisualEntry>
        {
            new ToolbarVisualEntry { label = "Dock", icon = GetSpriteByName("anchor") },
            new ToolbarVisualEntry { label = "Map", icon = GetSpriteByName("map") }
        };

        _toolbarVisuals[GameStates.Paused] = new List<ToolbarVisualEntry>
        {
            new ToolbarVisualEntry { label = "Test Paused", icon = GetSpriteByName("test") }
        };

        _toolbarVisuals[GameStates.InMenu] = new List<ToolbarVisualEntry>
        {
            new ToolbarVisualEntry { label = "Close menu", icon = GetSpriteByName("test") }
        };

    }*/

    /*public void UpdateToolBar(GameStates state)
    {
        //Debug.Log("Update toolbar called for sate: " + state);

        foreach (Transform child in toolBarContainer)
            Destroy(child.gameObject);

        if (!_toolbarVisuals.ContainsKey(state))
        {
            return;
        }

        foreach (var entry in _toolbarVisuals[state])
        {
            GameObject newEntry = Instantiate(toolBarItemPrefab, toolBarContainer);

            Transform textChild = newEntry.transform.Find("Txt_ToolBar");
            Transform imgChild = newEntry.transform.Find("Img_ToolBar");

            if (textChild == null || imgChild == null)
                continue;

            //textChild.GetComponent<TMPro.TextMeshProUGUI>().text = entry.label;
            //imgChild.GetComponent<Image>().sprite = entry.icon;

        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(toolBarContainer.GetComponent<RectTransform>());
    }/*


    /*private Sprite GetSpriteByName(string name)
    {
        //Sprite sprite = toolBarSprites.Find(s => s.name == name);

        if (sprite == null)
        {
            Debug.LogWarning("Sprite not found: " + name);
            return null;
        }

        return sprite;
    }*/

    public void UpdateToolBar(GameStates state)
    {
        //currently hardcoded will need to be changed based on the controlles attached to the computer or currently active.
        KeyType keyType = KeyType.Keyboard;

        //we're tracking the key state with a basic variable at the top
        //so we dont need to drop or destroy some things if not necessary
        //even if not it saves a bit performance :D
        if(currentKeyState == state && currentKeyType == keyType)
        {
            return;
        }
        //if we're not in the state we basically clear all buttons before we instantiate the new ones
        if(currentButtons.Count > 0)
        {
            currentButtons.Clear();
        }
        //TODO Keytype
        ToolBarStateEntry newEntry = toolBarStateEntries.Find(t => t.StateType == state && t.KeyType == keyType);

        foreach(ToolbarVisualEntry button in newEntry.Buttons)
        {
            GameObject newButton = Instantiate(toolBarItemPrefab, toolBarContainer);
            newButton.GetComponent<UIToolbarItem>().SetupButton(button.Sprite, button.Label, button.Key);
            currentButtons.Add(newButton);
        }
    }

    //Both of this function do the same, but look completely different it would be good for consistency to use the same approach :)
    public void ToggleDateMap()
    {
        //add the null check here as well :)
        if(dateMapContainer != null)
        {
            bool isActive = dateMapContainer.gameObject.activeSelf;
            dateMapContainer.gameObject.SetActive(!isActive);
        }
    }

    public void ToggleToolbar()
    {
        if (toolBarContainer != null)
        {
            toolBarContainer.gameObject.SetActive(toggleToolbar);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        ToggleToolbar();
    }
#endif

}
