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

    public void Setup(TrashMaterialData mat)
    {
        this.currentMaterial = mat;
        this.image.sprite = mat.sprite;
    }

    public void ResetSlot()
    {
        this.currentMaterial = null;
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
