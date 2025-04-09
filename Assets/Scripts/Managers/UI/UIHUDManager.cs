using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class ToolbarVisualEntry
{
    public string label;
    public Sprite icon;
}

public class UIHUDManager : MonoBehaviour
{
    [Header("Toolbar")]
    [SerializeField] private GameObject toolBarItemPrefab;
    [SerializeField] private Transform toolBarContainer;
    [SerializeField] private bool toggleToolbar = true;
    [SerializeField] private List<Sprite> toolBarSprites;

    [Header("DateMap")]
    [SerializeField] private Transform dateMapContainer;

    private Dictionary<GameStates, List<ToolbarVisualEntry>> _toolbarVisuals = new();


    public static UIHUDManager Instance;

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
        ToolbarSetup();
    }

    private void OnDisable()
    {
        GameStateManager.OnStateChanged -= UpdateToolBar;
    }

    private void ToolbarSetup()
    {
        Debug.Log("ToolbarSetup called");
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

    }

    public void UpdateToolBar(GameStates state)
    {
        Debug.Log("Update toolbar called for sate: " + state);

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

            textChild.GetComponent<TMPro.TextMeshProUGUI>().text = entry.label;
            imgChild.GetComponent<Image>().sprite = entry.icon;

        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(toolBarContainer.GetComponent<RectTransform>());
    }
    private Sprite GetSpriteByName(string name)
    {
        Sprite sprite = toolBarSprites.Find(s => s.name == name);

        if (sprite == null)
        {
            Debug.LogWarning("Sprite not found: " + name);
            return null;
        }

        return sprite;
    }

    public void ToggleDateMap()
    {
        bool isActive = dateMapContainer.gameObject.activeSelf;
        dateMapContainer.gameObject.SetActive(!isActive);
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
