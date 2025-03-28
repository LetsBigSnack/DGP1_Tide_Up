using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string trashName;
    public List<TrashMaterialData> materials = new List<TrashMaterialData>();
}
