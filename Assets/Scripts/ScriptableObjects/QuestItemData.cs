using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestItemData", menuName = "Scriptable Objects/QuestItemData")]
public class QuestItemData : ItemData
{
    public List<string> itemUses;

    public string GetUse()
    {
        if (itemUses == null || itemUses.Count == 0)
        {
            throw new ArgumentException("itemUses is empty or null!");
        }
        return itemUses[UnityEngine.Random.Range(0, itemUses.Count)];
    }
}
