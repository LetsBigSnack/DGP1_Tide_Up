using UnityEngine;

public enum TrashMaterialType
{
    Glass,
    Metal,
    Paper,
    Plastic,
    Wood
}

public class TrashMaterial : MonoBehaviour
{
    public TrashMaterialData trashMaterialData;
}
