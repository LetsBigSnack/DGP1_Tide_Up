using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSelectableFix : MonoBehaviour, ISelectHandler
{
    private Selectable m_selectable;

    // Use this for initialization
    void Awake()
    {
        m_selectable = GetComponent<Selectable>();
    }

    public void OnSelect(BaseEventData _data)
    {
        if (!m_selectable.interactable && _data is AxisEventData axisEvent)
        {
            var goBackSelect = m_selectable.FindSelectable(axisEvent.moveVector *= -1);
            StartCoroutine(DelaySelect(goBackSelect));
            return;
        }
    }

    private IEnumerator DelaySelect(Selectable select)
    {
        yield return new WaitForEndOfFrame();

        if (select != null || !select.gameObject.activeInHierarchy)
            select.Select();
        else
            Debug.LogWarning("Please make sure your explicit navigation is configured correctly.");
    }
}
