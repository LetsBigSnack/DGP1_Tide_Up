using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildManager : MonoBehaviour
{
    public static UIBuildManager Instance;

    private bool isOpen = false;

    [Header("SubMenu")]
    [SerializeField] private GameObject subMenu;

    [Header("Current Requirements")]
    private List<GameObject> _currentRequirements = new List<GameObject>();
    private List<TrashMaterialEntry> _currentMaterialsNeeded = new List<TrashMaterialEntry>();
    private BuildSpotInteractable _currentSpot;

    [Header("Rebuild Info")]
    [SerializeField] private Image buildImage;
    [SerializeField] private TextMeshProUGUI buildDescription;
    [SerializeField] private TextMeshProUGUI subText;
    [SerializeField] private Image buttonImage;

    [Header("Required Items")]
    [SerializeField] private GameObject reqItemPrefab;
    [SerializeField] private Transform reqItemParent;

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

    public bool IsOpen()
    {
        return isOpen;
    }

    public void OpenMenu()
    {
        subMenu.SetActive(true);
        GameStateManager.Instance.SetGameState(GameStates.Building);
        isOpen = true;
    }

    public void CloseMenu()
    {
        ClearMenu();
        isOpen = false;
        subMenu.SetActive(false);
    }

    private void AddRequirements(List<TrashMaterialEntry> requirements)
    {
        ClearMenu();

        if (requirements == null) return;

        foreach (TrashMaterialEntry entry in requirements)
        {
            GameObject newRequirement = Instantiate(reqItemPrefab, reqItemParent);
            newRequirement.GetComponent<UIBuildRequirementItem>().Setup(entry, MaterialIsAvailable(entry));
            _currentRequirements.Add(newRequirement);
        }
    }

    public void Setup(Sprite image = null, string description = "", string subtext = "", List<TrashMaterialEntry> requirements = null, BuildSpotInteractable spot = null)
    {
        if (image)
        {
            this.buildImage.sprite = image;
        }

        this.buildDescription.text = description;
        this.subText.text = subtext;
        this._currentMaterialsNeeded = requirements ?? new List<TrashMaterialEntry>();
        this._currentSpot = spot;

        AddRequirements(_currentMaterialsNeeded);
        CheckButtonActive();
        OpenMenu();
    }

    private bool MaterialIsAvailable(TrashMaterialEntry t)
    {
        return t.Amount <= InventoryManager.Instance.GetMaterialAmount(t.TrashMaterialData.type);
    }

    private void CheckButtonActive()
    {
        bool canBuild = true;
        Color alpha = buttonImage.color;

        foreach (TrashMaterialEntry t in _currentMaterialsNeeded)
        {
            int neededAmount = t.Amount;
            int currentAmount = InventoryManager.Instance.GetMaterialAmount(t.TrashMaterialData.type);

            if (neededAmount > currentAmount)
            {
                canBuild = false;
                break;
            }
        }

        alpha.a = canBuild ? 1f : 0.75f;
        buttonImage.color = alpha;
    }

    public void Build()
    {
        if (!_currentSpot.Build())
        {
            UI_ToastManager.Instance.SpawnToastMessage(
                ToastType.Important,
                "You do not have the necessary amount of materials to rebuild this spot!",
                null
            );
        }
        else
        {
            CloseMenu();
            GameStateManager.Instance.SetGameState(GameStates.PlayingCharacter);
            UI_ToastManager.Instance.SpawnToastMessage(
                ToastType.Important,
                "Well done! Spot has been rebuilt!",
                null
            );
        }
    }

    private void ClearMenu()
    {
        foreach (GameObject o in _currentRequirements)
        {
            Destroy(o);
        }
        _currentRequirements.Clear();
    }
}
