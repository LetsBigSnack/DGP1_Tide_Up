using Assets.Scripts.Data;
using TMPro;
using UnityEngine;

public class UIMapController : UIJournalSubMenu
{
    //TODO: Islands should know what is on them (houses, shops, etc)
    //Islands should know where their CenterPoint is for the camera

    public static UIMapController Instance;

    [Header("SubMenu")]
    [SerializeField] private GameObject mapMenu;

    [Header("Island Name")]
    [SerializeField] private TextMeshProUGUI islandName;

    public override void CloseMenu()
    {
        mapMenu.SetActive(false);
    }

    public override void OpenMenu()
    {
        mapMenu.SetActive(true);
    }

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

    private void SetName()
    {
        islandName.text = EnvironmentManager.Instance.GetCurrentIsland().IslandName;
    }
}
