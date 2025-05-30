using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEventSystemHelper : MonoBehaviour
{
    public static UIEventSystemHelper Instance;

    [SerializeField] private EventSystem eventsystem;

    public EventSystem EventSystemObj
    {
        get => eventsystem;
        set => eventsystem = value;
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        eventsystem = GetComponentInChildren<EventSystem>();
    }

    public void SetFirstSelectedItem(GameObject gameObject)
    {
        StartCoroutine(FirstFrameDelay(gameObject));
    }

    public IEnumerator FirstFrameDelay(GameObject gameObject)
    {
        yield return null;

        eventsystem.firstSelectedGameObject = gameObject;

        Button buttonInChildren = gameObject.GetComponentInChildren<Button>();
        if (buttonInChildren)
        {
            buttonInChildren.Select();
        }

        Button buttonInParent = gameObject.GetComponent<Button>();
        if (buttonInParent)
        {
            buttonInParent.Select();
        }

        Slider slider = gameObject.GetComponent<Slider>();
        if (slider)
        {
            slider.Select();
        }
    }
}
