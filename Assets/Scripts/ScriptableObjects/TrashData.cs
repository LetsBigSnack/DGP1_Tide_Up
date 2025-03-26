using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrashData", menuName = "Scriptable Objects/TrashData")]
public class TrashData : ScriptableObject
{
    public string trashName;
    public bool trashQual;
    public List<TrashMaterialData> materials = new List<TrashMaterialData>();
}
