using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "TrashMaterialData", menuName = "Scriptable Objects/TrashMaterialData")]
public class TrashMaterialData : ScriptableObject
{
    public TrashMaterialType type;
    public List<string> trashHints;
    public string materialName;
    
    public string GetHint()
    {
        if (trashHints == null || trashHints.Count == 0)
        {
            throw new ArgumentException("trashHints is empty or null!");
        }
        return trashHints[Random.Range(0, trashHints.Count)];  
    }
}
