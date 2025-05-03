using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemDetailsHelper : MonoBehaviour
{
    public static UIItemDetailsHelper Instance;

    [Header("Setup")]
    [SerializeField] private TextMeshProUGUI titel;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image image;
    [SerializeField] private Sprite baseSprite;

    [Header("Material Parent")]
    [SerializeField] private Transform matsParent;

    [Header("Material Prefab")]
    [SerializeField] private GameObject materialPrefab;
    [SerializeField] private GameObject emptyPrefab;

    private UIInventoryItem _currentSelectedItem;

    private List<GameObject> _trashMaterial = new List<GameObject>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        CreateMaterialIcons(null);
    }

    private void OnDisable()
    {
        RemoveMaterialIcons();
        _currentSelectedItem = null;
        ResetDescription();
    }

    public void SetupDescription(string titel, string description, Sprite image, List<TrashMaterialData> trash)
    {
        ResetDescription();
        this.titel.text = titel.ToUpper();
        this.description.text = description;
        this.image.enabled = true;
        this.image.sprite = image;
        CreateMaterialIcons(trash);
    }

    public void ResetDescription()
    {
        this.titel.text = "";
        this.description.text = "Nothing is selected.";
        this.image.sprite = baseSprite;
    }

    public void SetGameObjectAsSelected(UIInventoryItem item)
    {
        if(_currentSelectedItem == item)
        {
            return;
        }

        if(_currentSelectedItem == null)
        {
            _currentSelectedItem = item;
            item.ToggleIcon();
            return;
        }

        _currentSelectedItem.ToggleIcon();
        _currentSelectedItem = item;
        _currentSelectedItem.ToggleIcon();
    }

    private void CreateMaterialIcons(List<TrashMaterialData> trash)
    {
        RemoveMaterialIcons();

        if (trash != null)
        {
            foreach (TrashMaterialData mat in trash)
            {
                GameObject newMaterial = Instantiate(materialPrefab, matsParent);
                newMaterial.GetComponent<UIDescriptionMaterialItem>().Setup(mat.sprite);
                _trashMaterial.Add(newMaterial);
            }
        }

        if (_trashMaterial.Count < 4)
        {
            for (int i = _trashMaterial.Count; i < 4; i++)
            {
                GameObject newEmptyMaterial = Instantiate(emptyPrefab, matsParent);
                _trashMaterial.Add(newEmptyMaterial);
            }
        }
    }

    private void RemoveMaterialIcons()
    {
        if(_trashMaterial.Count <= 0) {
            return;
        }

        foreach(GameObject mat in _trashMaterial)
        {
            Destroy(mat);
        }
        _trashMaterial.Clear();
    }
}
