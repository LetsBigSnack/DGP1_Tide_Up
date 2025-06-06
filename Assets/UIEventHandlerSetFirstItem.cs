using UnityEngine;
using UnityEngine.UI;

public class UIEventHandlerSetFirstItem : MonoBehaviour
{
    private void OnEnable()
    {
        if(InputDeviceHelper.Instance.IsController() && gameObject.GetComponent<Selectable>().navigation.mode != Navigation.Mode.None)
        {
            UIEventSystemHelper.Instance?.EventSystemObj?.SetSelectedGameObject(null);
            UIEventSystemHelper.Instance?.SetFirstSelectedItem(gameObject);
            UIEventSystemHelper.Instance?.ForceLastValidSelection(gameObject);
        } 
    }

    private void LateUpdate()
    {
       if(!UIEventSystemHelper.Instance.LastValidSelectionExists() && InputDeviceHelper.Instance.IsController())
        {
            UIEventSystemHelper.Instance?.ForceLastValidSelection(gameObject);
        }
    }
}