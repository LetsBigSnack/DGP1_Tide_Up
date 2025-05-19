using UnityEngine;
using Data;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

[Serializable]
public class MaterialExchangeEntry
{
    public TrashMaterialType type;
    public Sprite sprite;
    public TextMeshProUGUI ownedText;
    public TextMeshProUGUI previewText;
}

public class UIMaterialExchangeController : UIShopSubMenu
{
    public static UIMaterialExchangeController Instance;

    [Header("SubMenu")]
    [SerializeField] private GameObject subMenu;

    [Header("Currently Picked Materials")]
    [SerializeField] private TrashMaterialType inputType = TrashMaterialType.Glass;
    [SerializeField] private TrashMaterialType outputType = TrashMaterialType.Metal;
    [SerializeField] private int exchangeAmount;
    [SerializeField] private TextMeshProUGUI exchangeAmountText;

    [Header("Current Output Preview")]
    [SerializeField] private Image inputImage;
    [SerializeField] private Image outputImage;

    [SerializeField] private List<MaterialExchangeEntry> materialEntries = new List<MaterialExchangeEntry>();

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
        InventoryManager.OnTrashMaterialChanged += UpdateEntries;
    }

    private void OnDisable()
    {
        InventoryManager.OnTrashMaterialChanged -= UpdateEntries;
    }

    public override void CloseMenu()
    {
        subMenu.SetActive(false);
    }

    public override void OpenMenu()
    {
        subMenu.SetActive(true);
        Setup();
    }

    private void Setup()
    {
        inputType = TrashMaterialType.Glass;
        outputType = TrashMaterialType.Metal;

        inputImage.sprite = ReturnEntrybyType(inputType).sprite;
        outputImage.sprite = ReturnEntrybyType(outputType).sprite;

        exchangeAmount = 0;
        UpdateExchangeAmountText();
        UpdateEntries(null);
    }

    private void UpdateExchangeAmountText()
    {
        exchangeAmountText.text = exchangeAmount.ToString();
    }

    private void UpdateEntries(List<TrashMaterialEntry> wallet)
    {
        if(wallet == null)
        {
            wallet = InventoryManager.Instance.GetWallet();
        }

        foreach(TrashMaterialEntry entry in wallet)
        {
            MaterialExchangeEntry matEntry = ReturnEntrybyType(entry.TrashMaterialData.type);
            matEntry.sprite = entry.TrashMaterialData.sprite;
            matEntry.ownedText.text = entry.Amount.ToString();
            matEntry.previewText.text = entry.Amount.ToString();
        }

        ChangeTextByTypeAndAmount();
    }

    private void ChangeTextByTypeAndAmount()
    {
        int ownedAmount = InventoryManager.Instance.GetMaterialAmount(inputType);
        ReturnEntrybyType(inputType).previewText.text = (ownedAmount - (exchangeAmount * MaterialExchangeManager.Instance.GetExchangeRate())).ToString();

        int previewAmount = InventoryManager.Instance.GetMaterialAmount(outputType);
        ReturnEntrybyType(outputType).previewText.text = (previewAmount + exchangeAmount).ToString();
    }

    private MaterialExchangeEntry ReturnEntrybyType(TrashMaterialType type)
    {
        return materialEntries.Find(t => t.type == type);
    }

    public void AddAmount()
    {
        if(MaterialExchangeManager.Instance.ReturnMaxExchangeAmount(inputType) < exchangeAmount + 1)
        {
            //error Message
            return;
        }

        exchangeAmount += 1;
        UpdateExchangeAmountText();
        ChangeTextByTypeAndAmount();
    }

    public void DecreaseAmount()
    {
        if(exchangeAmount -1 < 0)
        {
            return;
        }
        exchangeAmount -= 1;
        UpdateExchangeAmountText();
        ChangeTextByTypeAndAmount();
    }

    public void SetMinAmount()
    {
        exchangeAmount = 0;
        UpdateExchangeAmountText();
        ChangeTextByTypeAndAmount();
    }

    public void SetMaxAmount()
    {
        exchangeAmount = MaterialExchangeManager.Instance.ReturnMaxExchangeAmount(inputType);
        UpdateExchangeAmountText();
        ChangeTextByTypeAndAmount();
    }

    public void SetInputTypeByInt(int type)
    {
        if (inputType == (TrashMaterialType)type || outputType == (TrashMaterialType)type)
        {
            //Error Message here.
            return;
        }
        exchangeAmount = 0;
        UpdateExchangeAmountText();
        inputType = (TrashMaterialType)type;
        inputImage.sprite = ReturnEntrybyType(inputType).sprite;
        UpdateEntries(null);
    }

    public void SetOutputTypeByInt(int type)
    {
        if(inputType == (TrashMaterialType)type || outputType == (TrashMaterialType)type)
        {
            //Error Message here.
            return;
        }
        outputType = (TrashMaterialType)type;
        exchangeAmount = 0;
        UpdateExchangeAmountText();
        outputImage.sprite = ReturnEntrybyType(outputType).sprite;
        UpdateEntries(null);
    }
    public void Exchange()
    {
        if(exchangeAmount == 0)
        {
            return;
        }

        if(!MaterialExchangeManager.Instance.ExchangeMaterial(inputType, outputType, exchangeAmount))
        {
            //error message
            UI_ToastManager.Instance.SpawnToastMessage(ToastType.Important, "Something went wrong", "You cant do that now");
            return;
        }
        //notification manager
        Debug.Log("Material Exchanged");
        ResetAfterExchange();
        SoundManager.Instance.PlaySFX("Buy");
    }

    private void ResetAfterExchange()
    {
        exchangeAmount = 0;
        UpdateExchangeAmountText();
        UpdateEntries(null);
    }

}
