using Assets.Scripts.Data;
using UnityEngine;

public class UIQuestMenuController : UIJournalSubMenu
{
    [Header("SubMenu")]
    [SerializeField] private GameObject subMenu;

    public static UIQuestMenuController Instance;
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

    public override void CloseMenu()
    {
        subMenu.SetActive(false);
    }

    public override void OpenMenu()
    {
        subMenu.SetActive(true);
    }
}
