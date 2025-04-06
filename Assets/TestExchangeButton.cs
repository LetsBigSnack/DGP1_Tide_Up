using UnityEngine;

public class TestExchangeButton : MonoBehaviour
{
    [SerializeField] private int amount;
    [SerializeField] private TrashMaterialType typeToGet;
    [SerializeField] private TrashMaterialType typeToPay;

    private void Start()
    {
        InventoryManager.Instance.AddMaterial(TrashMaterialType.Glass, 10);
        InventoryManager.Instance.AddMaterial(TrashMaterialType.Paper, 10);
        InventoryManager.Instance.AddMaterial(TrashMaterialType.Metal, 10);
        InventoryManager.Instance.AddMaterial(TrashMaterialType.Wood, 10);
        InventoryManager.Instance.AddMaterial(TrashMaterialType.Plastic, 10);
    }
    public void ExchangeMaterial()
    {
        MaterialExchangeManager.Instance.ExchangeMaterial(typeToPay, typeToGet, amount);
    }
}
