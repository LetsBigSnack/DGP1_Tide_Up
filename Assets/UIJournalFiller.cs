using UnityEngine;

public class UIJournalFiller : MonoBehaviour
{
    [SerializeField] private JournalType type;
    [SerializeField] private bool isRight;

    [SerializeField] private UIJournalFiller prevBookMark;
    [SerializeField] private UIJournalFiller nextBookMark;

    [SerializeField] private UIJournalBookMarkItem currentItem;

    public JournalType Type
    {
        get => type;
        set => type = value;
    }

    public bool IsRight
    {
        get => isRight;
        set => isRight = value;
    }

    public UIJournalBookMarkItem CurrentItem
    {
        get => currentItem;
        set => currentItem = value;
    }

    public UIJournalFiller GetPreviousBookmark()
    {
        return prevBookMark;
    }

    public UIJournalFiller GetNextBookmark()
    {
        return nextBookMark;
    }
}
