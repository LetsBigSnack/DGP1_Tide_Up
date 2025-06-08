using System;
using System.Collections.Generic;
using UnityEngine;


public enum ButtonInputType
{
    Interact,
    Move,
    Close,
    Inventory,
    Recipes,
    Quests,
    Friendbook,
    Calender,
    Map,
    Journal,
    Prev,
    Next
}

[Serializable]
public class ButtonEntry
{

    [SerializeField]
    private ButtonInputType type;
    [SerializeField]
    private string label;
    [SerializeField]
    private string key;
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private bool notNeeded;

    public ButtonInputType Type
    {
        get { return type; }
        set { type = value; }
    }

    public string Label
    {
        get { return label; }
        set { label = value; }
    }

    public string Key
    {
        get { return key; }
        set { key = value; }
    }

    public Sprite Sprite
    {
        get { return sprite; }
        set { sprite = value; }
    }

    public bool NotNeeded
    {
        get { return notNeeded; }
        set { notNeeded = value; }
    }
}

[CreateAssetMenu(fileName = "ToolBarSetting", menuName = "Scriptable Objects/ToolBarSetting")]
public class ToolBarSetting : ScriptableObject
{
    [SerializeField] private DeviceType type;

    [Header("Player")]
    [SerializeField] private ButtonEntry journal;
    [SerializeField] private ButtonEntry toggleMove;

    [Header("Journal")]
    [SerializeField] private ButtonEntry inventory;
    [SerializeField] private ButtonEntry recipes;
    [SerializeField] private ButtonEntry quests;
    [SerializeField] private ButtonEntry friendbook;
    [SerializeField] private ButtonEntry calender;
    [SerializeField] private ButtonEntry map;

    [Header("General")]
    [SerializeField] private ButtonEntry nextTab;
    [SerializeField] private ButtonEntry prevTab;
    [SerializeField] private ButtonEntry interact;
    [SerializeField] private ButtonEntry close;

    public List<ButtonEntry> ButtonEntries
    {
        get
        {
            return new List<ButtonEntry>
        {
            journal,
            toggleMove,
            inventory,
            recipes,
            quests,
            friendbook,
            calender,
            map,
            interact,
            close,
            nextTab,
            prevTab
        };
        }
    }
    public DeviceType Type
    {
        get { return type; }
        set { type = value; }
    }

    public ButtonEntry Journal
    {
        get { return journal; }
        set { journal = value; }
    }

    public ButtonEntry ToggleMove
    {
        get { return toggleMove; }
        set { toggleMove = value; }
    }

    public ButtonEntry Inventory
    {
        get { return inventory; }
        set { inventory = value; }
    }

    public ButtonEntry Recipes
    {
        get { return recipes; }
        set { recipes = value; }
    }

    public ButtonEntry Quests
    {
        get { return quests; }
        set { quests = value; }
    }

    public ButtonEntry Friendbook
    {
        get { return friendbook; }
        set { friendbook = value; }
    }

    public ButtonEntry Calender
    {
        get { return calender; }
        set { calender = value; }
    }

    public ButtonEntry Map
    {
        get { return map; }
        set { map = value; }
    }

    public ButtonEntry Interact
    {
        get { return interact; }
        set { interact = value; }
    }

    public ButtonEntry Close
    {
        get { return close; }
        set { close = value; }
    }

    public ButtonEntry NextTab
    {
        get { return nextTab; }
        set { nextTab = value; }
    }

    public ButtonEntry PrevTab
    {
        get { return prevTab; }
        set { prevTab = value; }
    }
}