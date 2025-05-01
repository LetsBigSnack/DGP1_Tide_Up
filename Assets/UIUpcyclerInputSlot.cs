using UnityEngine;
using Data;
using UnityEngine.UI;

public class UIUpcyclerInputSlot : MonoBehaviour
{
    [SerializeField] private TrashMaterialData currentMaterial;
    [SerializeField] private Image image;

    public TrashMaterialData CurrentMaterial
    {
        get { return currentMaterial; }
        set { currentMaterial = value; }
    }

    private void Start()
    {
        image.enabled = false;
    }

    public void Setup(TrashMaterialData mat)
    {
        this.currentMaterial = mat;
        this.image.sprite = mat.sprite;
        this.image.enabled = true;
    }

    public void ResetSlot()
    {
        this.currentMaterial = null;
        this.image.enabled = false;
        this.image.sprite = null;
    }

    public void RemoveItem()
    {
        if (currentMaterial != null)
        {
            UIUpcyclerController.Instance.RemoveMaterial(this);
            ResetSlot();
        }
    }
}
