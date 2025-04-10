using Data;
using UnityEngine;

public class DevTestHelper : MonoBehaviour
{
    
    public static TrashItemInstance TestGetTrashFromBox(int itemIndex)
    {
        TideUpBox currBox = TideUpBoxManager.Instance.GetTideUpBox(0);

        if (currBox.BoxInventory.Count == 0)
        {
            return null;
        }

        TrashItemInstance itemToCollect = currBox.BoxInventory[itemIndex];
        return itemToCollect;
    }
}
