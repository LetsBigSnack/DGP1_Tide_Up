using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Data;
using System;

public enum ReUpcyclerType
{
    Closed,
    Recipies,
    Recycler,
    Upcycler
}

public class UIReUpcycleManager : MonoBehaviour
{
    public static UIReUpcycleManager Instance;

    [Header("CurrentState")]
    [SerializeField] private ReUpcyclerType currentOpenType;

    [Header("Recycle/Upcycle")]
    [SerializeField] private List<UIReUpCyclerSubMenu> reUpcyclerSubMenues;

    public static event Action<ReUpcyclerType> OnReUpcycleStateChange;

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

    public ReUpcyclerType GetCurrentState()
    {
        return currentOpenType;
    }

    public void SwitchState(ReUpcyclerType state)
    {
        if (state == ReUpcyclerType.Closed)
        {
            CloseAllMenues();
            return;
        }
        CloseAllMenues();
        currentOpenType = state;
        OnReUpcycleStateChange?.Invoke(state);
        OpenMenuByType(state);
        GameStateManager.Instance.SetGameState(GameStates.InMenu);
    }

    public void CloseAllMenues()
    {
        foreach (UIReUpCyclerSubMenu menu in reUpcyclerSubMenues)
        {
            menu.CloseMenu();
        }
        currentOpenType = ReUpcyclerType.Closed;
        GameStateManager.Instance.SetGameState(GameStates.PlayingCharacter);
    }

    public void OpenMenuByType(ReUpcyclerType type)
    {
        reUpcyclerSubMenues.Where(m => m.GetComponent<UIReUpCyclerSubMenu>().ReUpCyclerMenuType == type).FirstOrDefault().OpenMenu();
    }

    public void SwitchStateByInt(int state)
    {
        SwitchState((ReUpcyclerType)state);
    }
}
