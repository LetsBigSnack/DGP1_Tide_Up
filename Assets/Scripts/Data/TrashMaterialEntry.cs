using UnityEngine;

public class TrashMaterialEntry
{
    public TrashMaterialData trashMaterialData;
    public int amount;

    public TrashMaterialEntry(TrashMaterialData material, int amount)
    {
        this.trashMaterialData = material;
        this.amount = amount;
    }
}
