using UnityEngine;
using Data;

public class MaterialExchangeManager : MonoBehaviour
{
    public static MaterialExchangeManager Instance;

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

    public void ExchangeMaterial(TrashMaterialType materialTypeToExchange, TrashMaterialType materialTypeToGet, int amount)
    {
        if(InventoryManager.Instance.HasSpaceForMaterial(materialTypeToGet, amount))
        {
            Debug.Log("You've reached the max amount of " + materialTypeToGet);
        }

        if(InventoryManager.Instance.GetMaterialAmount(materialTypeToExchange) < (amount * 3))
        {
            Debug.Log("You're missing " + ((amount * 3) - InventoryManager.Instance.GetMaterialAmount(materialTypeToExchange)) + " " + materialTypeToExchange +  " materials. To do that!");
            return;
        }
        InventoryManager.Instance.RemoveMaterial(materialTypeToExchange, amount * 3);
        InventoryManager.Instance.AddMaterial(materialTypeToGet, amount);
        Debug.Log("You've exchanged " + amount*3 + " " + materialTypeToExchange + " for " + amount + " " + materialTypeToGet);
    }
}
