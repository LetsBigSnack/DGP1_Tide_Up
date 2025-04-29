using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Data;

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
        currentOpenType = type;
        reUpcyclerSubMenues.Where(m => m.GetComponent<UIReUpCyclerSubMenu>().ReUpCyclerMenuType == type).FirstOrDefault().OpenMenu();
    }

    public void SwitchStateByInt(int state)
    {
        SwitchState((ReUpcyclerType)state);
    }
}
