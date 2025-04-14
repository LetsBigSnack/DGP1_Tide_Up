using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIWalletHelper : MonoBehaviour
{
    public static UIWalletHelper Instance;

    [SerializeField] private TextMeshProUGUI paperText;
    [SerializeField] private TextMeshProUGUI glassText;
    [SerializeField] private TextMeshProUGUI metalText;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI plasticText;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        UpdateWallet(InventoryManager.Instance.GetWallet());
        InventoryManager.OnTrashMaterialChanged += UpdateWallet;
    }

    private void OnDisable()
    {
        InventoryManager.OnTrashMaterialChanged -= UpdateWallet;
    }

    private void UpdateWallet(List<TrashMaterialEntry> materialdata)
    {
        foreach(TrashMaterialEntry mat in materialdata)
        {
            SelectTextField(mat.TrashMaterialData.type, mat.Amount.ToString());
        }
    }

    private void SelectTextField(TrashMaterialType type, string amount)
    {
        switch (type)
        {
            case TrashMaterialType.Glass:
                glassText.text = amount;
                break;
            case TrashMaterialType.Metal:
                metalText.text = amount;
                break;
            case TrashMaterialType.Paper:
                paperText.text = amount;
                break;
            case TrashMaterialType.Plastic:
                plasticText.text = amount;
                break;
            case TrashMaterialType.Wood:
                woodText.text = amount;
                break;
        }
    }
}
