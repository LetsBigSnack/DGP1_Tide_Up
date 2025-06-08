using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectAutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scrollSpeed = 10f;
    private bool mouseOver = false;

    private List<Selectable> m_Selectables = new List<Selectable>();
    private ScrollRect m_ScrollRect;

    private Vector2 m_NextScrollPosition = Vector2.up;
    private Vector2 inputVector;

    void Awake()
    {
        m_ScrollRect = GetComponent<ScrollRect>();
    }

    void Start()
    {
        RefreshSelectables();
        ScrollToSelected(true);
    }

    void Update()
    {
        InputScroll();

        if (!mouseOver)
        {
            m_ScrollRect.normalizedPosition = Vector2.Lerp(
                m_ScrollRect.normalizedPosition,
                m_NextScrollPosition,
                scrollSpeed * Time.unscaledDeltaTime
            );
        }
        else
        {
            m_NextScrollPosition = m_ScrollRect.normalizedPosition;
        }
    }

    void InputScroll()
    {
        RefreshSelectables();

        inputVector = Gamepad.current?.leftStick.ReadValue() ?? KeyboardNavigationInput();

        if (m_Selectables.Count > 0 && inputVector != Vector2.zero)
        {
            ScrollToSelected(false);
        }
    }

    Vector2 KeyboardNavigationInput()
    {
        Vector2 nav = Vector2.zero;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            nav.y = 1;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            nav.y = -1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            nav.x = 1;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            nav.x = -1;
        return nav;
    }

    void RefreshSelectables()
    {
        m_Selectables.Clear();
        m_ScrollRect.content.GetComponentsInChildren(true, m_Selectables);
    }

    void ScrollToSelected(bool quickScroll)
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;

        if (current == null && m_Selectables.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(m_Selectables[0].gameObject);
            current = EventSystem.current.currentSelectedGameObject;
        }

        Selectable selectedElement = current ? current.GetComponent<Selectable>() : null;
        int selectedIndex = selectedElement ? m_Selectables.IndexOf(selectedElement) : -1;

        if (selectedIndex > -1 && m_Selectables.Count > 1)
        {
            float normalizedY = 1f - (selectedIndex / ((float)m_Selectables.Count - 1));
            Vector2 targetPosition = new Vector2(0, normalizedY);

            if (quickScroll)
                m_ScrollRect.normalizedPosition = targetPosition;

            m_NextScrollPosition = targetPosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        ScrollToSelected(false);
    }
}
