using UnityEngine;
using Data;

public class MaterialExchangeManager : MonoBehaviour
{
    public static MaterialExchangeManager Instance;
    [SerializeField] private int exchangeRate = 3;


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

    public int GetExchangeRate()
    {
        return exchangeRate;
    }

    public bool ExchangeMaterial(TrashMaterialType materialTypeToExchange, TrashMaterialType materialTypeToGet, int amount)
    {
        if(!InventoryManager.Instance.HasSpaceForMaterial(materialTypeToGet, amount))
        {
            Debug.Log("You've reached the max amount of " + materialTypeToGet);
            return false;
        }

        if(InventoryManager.Instance.GetMaterialAmount(materialTypeToExchange) < (amount * exchangeRate))
        {
            Debug.Log("You're missing " + ((amount * exchangeRate) - InventoryManager.Instance.GetMaterialAmount(materialTypeToExchange)) + " " + materialTypeToExchange +  " materials. To do that!");
            return false;
        }
        InventoryManager.Instance.RemoveMaterial(materialTypeToExchange, amount * exchangeRate);
        InventoryManager.Instance.AddMaterial(materialTypeToGet, amount);
        Debug.Log("You've exchanged " + amount*exchangeRate + " " + materialTypeToExchange + " for " + amount + " " + materialTypeToGet);
        return true;
    }

    public int ReturnMaxExchangeAmount(TrashMaterialType input)
    {
        float amount = InventoryManager.Instance.GetMaterialAmount(input) / exchangeRate;

        return Mathf.FloorToInt(amount);
    }
}
