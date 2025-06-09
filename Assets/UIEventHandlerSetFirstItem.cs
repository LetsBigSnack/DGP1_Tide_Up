using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIEventHandlerSetFirstItem : MonoBehaviour
{
    private void OnEnable()
    {
        if(InputDeviceHelper.Instance.IsController() && gameObject.GetComponent<Selectable>().navigation.mode != Navigation.Mode.None)
        {
            StartCoroutine(SkipFirstFrame());
        } 
    }

    private void LateUpdate()
    {
       if(!UIEventSystemHelper.Instance.LastValidSelectionExists() && InputDeviceHelper.Instance.IsController())
        {
            UIEventSystemHelper.Instance?.ForceLastValidSelection(gameObject);
        }
    }

    private IEnumerator SkipFirstFrame()
    {
        yield return null;

        UIEventSystemHelper.Instance?.EventSystemObj?.SetSelectedGameObject(null);
        UIEventSystemHelper.Instance?.SetFirstSelectedItem(gameObject);
        UIEventSystemHelper.Instance?.ForceLastValidSelection(gameObject);
    }
}