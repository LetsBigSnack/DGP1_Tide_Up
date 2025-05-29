using UnityEngine;
using Data;
using UnityEngine.UI;

public class UIUpcyclerInputSlot : MonoBehaviour
{
    [SerializeField] private TrashMaterialData currentMaterial;
    [SerializeField] private Image image;
    private Button _button;

    public TrashMaterialData CurrentMaterial
    {
        get { return currentMaterial; }
        set { currentMaterial = value; }
    }

    private void Start()
    {
        image.enabled = false;
        _button = GetComponentInChildren<Button>();
        _button.interactable = false;
        Navigation nav = _button.navigation;
        nav.mode = Navigation.Mode.None;
        _button.navigation = nav;
    }

    public void Setup(TrashMaterialData mat)
    {
        this.currentMaterial = mat;
        this.image.sprite = mat.sprite;
        this.image.enabled = true;
        _button.interactable = true;
        Navigation nav = _button.navigation;
        nav.mode = Navigation.Mode.Automatic;
        _button.navigation = nav;
    }

    public void ResetSlot()
    {
        this.currentMaterial = null;
        this.image.enabled = false;
        this.image.sprite = null;
        if (!_button)
        {
            _button = GetComponentInChildren<Button>();
        }
        _button.interactable = false;
        Navigation nav = _button.navigation;
        nav.mode = Navigation.Mode.None;
        _button.navigation = nav;
    }

    public void RemoveItem()
    {
        if (currentMaterial != null)
        {
            UIUpcyclerController.Instance.RemoveMaterial(this);
            ResetSlot();
        }
        else
        {
            SoundManager.Instance.PlaySFX("Error");
        }
    }
}
