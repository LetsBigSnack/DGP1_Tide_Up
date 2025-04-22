using UnityEngine;

public class UIJournalBookMarkItem : MonoBehaviour
{
    [SerializeField] private JournalType type;

    public void OnClick()
    {
        UIJournalManager.Instance.SwitchState(type);
    }
}
