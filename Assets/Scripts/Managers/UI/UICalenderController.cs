using UnityEngine;
using Assets.Scripts.Data;

public class UICalenderController : UIJournalSubMenu
{

    public static UICalenderController Instance;
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
        
    }

    public override void OpenMenu()
    {
        
    }
}
