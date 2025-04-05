using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestItemData", menuName = "Scriptable Objects/QuestItemData")]
public class QuestItemData : ItemData
{
    public List<string> itemUses;

    public string GetUse()
    {
        if (itemUses == null)
        {
            throw new System.ArgumentNullException("itemUses");
        }
        return itemUses[UnityEngine.Random.Range(0, itemUses.Count)];
    }
}
