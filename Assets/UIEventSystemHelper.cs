using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEventSystemHelper : MonoBehaviour
{
    public static UIEventSystemHelper Instance;

    [SerializeField] private EventSystem eventsystem;
    [SerializeField] private GameObject currentSelectedObj;
    [SerializeField] private GameObject lastValidSelection;
    [SerializeField] private int maxStorage;
    private List<GameObject> _lastValidSelections = new List<GameObject>();
    public EventSystem EventSystemObj
    {
        get => eventsystem;
        set => eventsystem = value;
    }

    private void OnEnable()
    {
        GameStateManager.OnStateChanged += ClearLastValidButtons;
    }

    private void OnDisable()
    {
        GameStateManager.OnStateChanged -= ClearLastValidButtons;
    }

    public void ClearLastValidButtons(GameStates state)
    {
        if(state == GameStates.PlayingCharacter)
        {
            _lastValidSelections.Clear();
        }
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

    public void SetLastSelectedItem()
    {
        StartCoroutine(FirstFrameDelay(lastValidSelection));
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

    public bool LastValidSelectionExists()
    {
        return lastValidSelection != null;
    }

    public void SetLastValidSelection()
    {
        lastValidSelection = _lastValidSelections.Where(s => s != null && s != currentSelectedObj).FirstOrDefault();
    }

    public void UpdateValidSelection()
    {
        _lastValidSelections = _lastValidSelections.Where(s => s != null).ToList();
    }

    private void FixedUpdate()
    {
        if(eventsystem.currentSelectedGameObject != null && currentSelectedObj != eventsystem.currentSelectedGameObject && InputDeviceHelper.Instance.IsController())
        {
            currentSelectedObj = eventsystem.currentSelectedGameObject;
            eventsystem.SetSelectedGameObject(currentSelectedObj);
            _lastValidSelections.Add(currentSelectedObj);
            SetLastValidSelection(); 
        }
        else
        {
            if(eventsystem.currentSelectedGameObject == null && InputDeviceHelper.Instance.IsController())
            {
                UpdateValidSelection();
                eventsystem.SetSelectedGameObject(lastValidSelection);
            }
        }
    }
}
