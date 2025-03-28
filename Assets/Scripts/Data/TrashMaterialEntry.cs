using System.Collections.Generic;
using UnityEngine;

public class TrashMaterialEntry
{
    private TrashMaterialData trashMaterialData;
    private int amount;

    public TrashMaterialEntry(TrashMaterialData material, int amount)
    {
        this.trashMaterialData = material;
        this.amount = amount;
    }
    public TrashMaterialData TrashMaterialData
    {
        get { return trashMaterialData; }
        set { trashMaterialData = value; }
    }
    public int Amount
    {
        get { return amount; }
        set { amount = value; }
    }
}
