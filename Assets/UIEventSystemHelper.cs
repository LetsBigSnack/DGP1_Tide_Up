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
    [SerializeField] private float delayFirstInput = 0.25f;
    private List<GameObject> _lastValidSelections = new List<GameObject>();
    public EventSystem EventSystemObj
    {
        get => eventsystem;
        set => eventsystem = value;
    }

    private void OnEnable()
    {
        GameStateManager.OnStateChanged += ClearLastValidButtons;
        UIJournalManager.OnJournalStateChanged += ClearSelectedItemOnJournalChange;
    }

    private void OnDisable()
    {
        GameStateManager.OnStateChanged -= ClearLastValidButtons;
        UIJournalManager.OnJournalStateChanged -= ClearSelectedItemOnJournalChange;
    }

    private void ClearSelectedItemOnJournalChange(JournalType type)
    {
        currentSelectedObj = null;
        eventsystem?.SetSelectedGameObject(null);
    }

    public void ClearLastValidButtons(GameStates state, DeviceType type)
    { 
        if (state == GameStates.PlayingCharacter)
        {
            _lastValidSelections.Clear();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        yield return new WaitForSeconds(delayFirstInput);

        eventsystem.firstSelectedGameObject = gameObject;
         eventsystem.SetSelectedGameObject(gameObject);

        if (gameObject == null)
        {
            yield break;
        }

        Button buttonInChildren = gameObject?.GetComponentInChildren<Button>();
        if (buttonInChildren)
        {
            buttonInChildren.Select();
            yield break;
        }

        Slider slider = gameObject?.GetComponent<Slider>();
        if (slider)
        {
            slider.Select();
            yield break;
        }
    }

    public bool LastValidSelectionExists()
    {
        return lastValidSelection != null;
    }

    public void UpdateValidSelection()
    {
        _lastValidSelections = _lastValidSelections
            .Where(s => s != null && s.GetComponent<Selectable>()?.interactable == true && s.GetComponent<Selectable>()?.navigation.mode != Navigation.Mode.None && s.activeInHierarchy)
            .Distinct()
            .ToList();
    }

    public void SetLastValidSelection()
    {
        lastValidSelection = _lastValidSelections
            .Where(s => s != null && s != currentSelectedObj && s.GetComponent<Selectable>()?.navigation.mode != Navigation.Mode.None && s.activeInHierarchy)
            .FirstOrDefault();
    }

    public void ForceLastValidSelection(GameObject gameObject)
    {
        lastValidSelection = gameObject;
    }

    private void LateUpdate()
    {
        if (!InputDeviceHelper.Instance.IsController())
            return;

        GameObject selected = eventsystem.currentSelectedGameObject;

        // Case 1: Controller selected object is not interactable
        if (selected != null && !selected.GetComponent<Selectable>().interactable)
        {
            UpdateValidSelection();
            if (LastValidSelectionExists())
            {
                eventsystem.SetSelectedGameObject(lastValidSelection);
            }
        }

        // Case 2: Selection changed
        else if (selected != null && selected != currentSelectedObj)
        {
            currentSelectedObj = selected;

            if (!_lastValidSelections.Contains(currentSelectedObj) && currentSelectedObj.GetComponent<Selectable>()?.interactable == true)
            {
                _lastValidSelections.Add(currentSelectedObj);
            }

            SetLastValidSelection();
        }

        // Case 3: Nothing selected, but should be
        else if (selected == null)
        {
            UpdateValidSelection();
            if (LastValidSelectionExists())
            {
                eventsystem.SetSelectedGameObject(lastValidSelection);
            }
        }
    }
}
