using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrashMaterialData", menuName = "Scriptable Objects/TrashMaterialData")]
public class TrashMaterialData : ScriptableObject
{
    public TrashMaterialType type;
    public List<string> trashHints;
    public string materialName;

    public string GetHint()
    {
        if (trashHints == null)
        {
            throw new System.ArgumentNullException("trashHints");
        }
        return trashHints[UnityEngine.Random.Range(0, trashHints.Count)];    
    }
}
