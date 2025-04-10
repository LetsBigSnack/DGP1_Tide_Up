using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Data;

public enum ReUpcyclerType
{
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

    public void SwitchState(ReUpcyclerType state)
    {
        CloseAllMenues();
        OpenMenuByType(state);
        currentOpenType = state;
    }

    public void CloseAllMenues()
    {
        foreach (UIReUpCyclerSubMenu menu in reUpcyclerSubMenues)
        {
            menu.CloseMenu();
        }
    }

    public void OpenMenuByType(ReUpcyclerType type)
    {
        reUpcyclerSubMenues.Where(m => m.GetComponent<UIReUpCyclerSubMenu>().ReUpCyclerMenuType == type).FirstOrDefault().OpenMenu();
    }

}
