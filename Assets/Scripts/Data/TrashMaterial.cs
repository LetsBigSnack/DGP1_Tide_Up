using UnityEngine;

public enum TrashMaterialType
{
    Glass,
    Metal,
    Paper,
    Plastic,
    Wood
}

public class TrashMaterial
{
    public int amount;
    public TrashMaterialType type;
}
