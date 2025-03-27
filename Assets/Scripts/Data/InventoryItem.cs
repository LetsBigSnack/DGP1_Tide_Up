using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public TrashData trashData;
    public InventoryItem(TrashData trashData)
    {
        this.trashData = trashData;
    }
}
