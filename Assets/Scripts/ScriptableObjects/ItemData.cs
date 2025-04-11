using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string title;
    public string description;
    public Sprite sprite;
    [SerializeField] private List<TrashMaterialData> materials = new List<TrashMaterialData>();

    public List<TrashMaterialData> Materials
    {
        get => materials;
        set => materials = value;
    }
}
