using UnityEngine;

public class DevTestHelper : MonoBehaviour
{
    
    public static TrashData TestGetTrashFromBox(int itemIndex)
    {
        TideUpBox currBox = TideUpBoxManager.Instance.GetTideUpBox(0);

        if (currBox.BoxInventory.Count == 0)
        {
            return null;
        }

        TrashData itemToCollect = currBox.BoxInventory[itemIndex];
        return itemToCollect;
    }
}
