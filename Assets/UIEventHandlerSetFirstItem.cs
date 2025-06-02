using UnityEngine;

public class UIEventHandlerSetFirstItem : MonoBehaviour
{
    private void OnEnable()
    {
        UIEventSystemHelper.Instance.SetFirstSelectedItem(gameObject);
    }

    private void Update()
    {
        if (UIEventSystemHelper.Instance.EventSystemObj.currentSelectedGameObject == null && UIEventSystemHelper.Instance.EventSystemObj.lastValidSelection.Count <= 0)
        {
            UIEventSystemHelper.Instance.SetFirstSelectedItem(gameObject);
        }
    }
}
