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

    [Header("Material Parent")]
    [SerializeField] private Transform matsParent;

    [Header("Material Prefab")]
    [SerializeField] private GameObject materialPrefab;

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

    private void OnDisable()
    {
        RemoveMaterialIcons();
    }

    public void SetupDescription(string titel, string description, Sprite image, List<TrashMaterialData> trash)
    {
        this.titel.text = titel;
        this.description.text = description;
        this.image.sprite = image;
        CreateMaterialIcons(trash);
    }

    public void ResetDescription()
    {
        this.titel.text = "";
        this.description.text = "";
        this.image.sprite = null;
        RemoveMaterialIcons();
    }

    private void CreateMaterialIcons(List<TrashMaterialData> trash)
    {
        if(trash == null)
        {
            return;
        }

        foreach(TrashMaterialData mat in trash)
        {
            GameObject newMaterial = Instantiate(materialPrefab, matsParent);
            newMaterial.GetComponent<UIDescriptionMaterialItem>().Setup(mat.sprite);
            _trashMaterial.Add(newMaterial);
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
