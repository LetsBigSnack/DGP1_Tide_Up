using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIUpcyclerSubRecipeEntryItem : MonoBehaviour
{
    [SerializeField] private GameObject materialPrefab;
    [SerializeField] private Transform materialParent;
    [SerializeField] private Image image;
    [SerializeField] private Sprite activeBtn;
    [SerializeField] private Sprite inactiveBtn;

    private List<GameObject> _currentMaterials = new List<GameObject>();
    private List<TrashMaterialData> _materials = new List<TrashMaterialData>();

    private Dictionary<TrashMaterialData, int> _amountNeeded = new Dictionary<TrashMaterialData, int>();

    private void OnEnable()
    {
        InventoryManager.OnTrashMaterialChanged += UpdateMaterials;
    }

    private void OnDisable()
    {
        InventoryManager.OnTrashMaterialChanged -= UpdateMaterials;
    }

    public void Setup(List<TrashMaterialData> materials)
    {
        this._materials = materials.OrderBy(t => t.type).ToList();
        UpdateMaterials();
    }

    public void UpdateMaterials(List<TrashMaterialEntry> currentInventory = null)
    {
        ClearCurrentMaterials();
        CreateMaterials();
        SpawnMaterials();
        UpdateButton();
    }

    private void UpdateButton()
    {
        if (CanMaterialsBeAddedToUpcycler())
        {
            image.sprite = activeBtn;
            return;
        }
        image.sprite = inactiveBtn;
    }

    private void CreateMaterials()
    {
        _amountNeeded.Clear();

        foreach (TrashMaterialData t in _materials)
        {
            if (!_amountNeeded.ContainsKey(t))
            {
                _amountNeeded.Add(t, 1);
            }
            else
            {
                _amountNeeded[t] += 1;
            }
        }
    }

    private void SpawnMaterials()
    {
        int count = 1;
        TrashMaterialData curMat = null;
        TrashMaterialData prevMat = null;

        foreach (TrashMaterialData t in _materials)
        {
            if(curMat == null || t.type != curMat.type)
            {
                if(curMat != null)
                {
                    prevMat = curMat;
                }
                curMat = t;
                count = 1;
            }
            else
            {
                count++;
            }

            GameObject mat = Instantiate(materialPrefab, materialParent);
            Image matImage = null;
            foreach (var img in mat.GetComponentsInChildren<Image>(true))
            {
                if (img.gameObject != mat)
                {
                    matImage = img;
                    break;
                }
            }
            matImage.sprite = t.sprite;

            if(InventoryManager.Instance.GetMaterialAmount(t.type) < count)
            {
                
                matImage.color = new Color(matImage.color.r, matImage.color.g, matImage.color.b, 0.5f);
            }

            _currentMaterials.Add(mat);
        }
    }

    private bool CanMaterialsBeAddedToUpcycler()
    {
        bool hasEnough = false;

        foreach(TrashMaterialData t in _amountNeeded.Keys)
        {
            if(InventoryManager.Instance.GetMaterialAmount(t.type) >= _amountNeeded[t])
            {
                hasEnough = true;
            }
            else
            {
                hasEnough = false;
                break;
            }
        }
        return hasEnough;
    }

    public void PutMaterialsIntoUpcycler()
    {
        if (CanMaterialsBeAddedToUpcycler())
        {
            UIUpcyclerController.Instance.RemoveAllMaterials();

            foreach (TrashMaterialData t in _materials)
            {
                UIUpcyclerController.Instance.AddMaterial(t);
            }
            UpdateMaterials();
        }
    }

    private void ClearCurrentMaterials()
    {
        if(_currentMaterials.Count <= 0)
        {
            return;
        }

        foreach(GameObject o in _currentMaterials)
        {
            Destroy(o);
        }

        _currentMaterials.Clear();
    }

    public void RemoveSub()
    {
        Destroy(gameObject);
    }
}
