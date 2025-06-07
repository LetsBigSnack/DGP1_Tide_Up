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

    [System.Serializable]
    public struct ValidSelection
    {
        public GameObject obj;
        public float time;

        public ValidSelection(GameObject obj, float time)
        {
            this.obj = obj;
            this.time = time;
        }
    }

    private List<ValidSelection> _lastValidSelections = new List<ValidSelection>();

    public EventSystem EventSystemObj
    {
        get => eventsystem;
        set => eventsystem = value;
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

    private void Start()
    {
        eventsystem = GetComponentInChildren<EventSystem>();
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

    public void SetFirstSelectedItem(GameObject gameObject)
    {
        StartCoroutine(FirstFrameDelay(gameObject));
    }

    public void SetLastSelectedItem()
    {
        StartCoroutine(FirstFrameDelay(lastValidSelection));
    }

    private IEnumerator FirstFrameDelay(GameObject gameObject)
    {
        yield return new WaitForSeconds(delayFirstInput);

        if (gameObject == null || !gameObject.activeInHierarchy)
            yield break;

        eventsystem.firstSelectedGameObject = gameObject;
        eventsystem.SetSelectedGameObject(gameObject);

        Button buttonInChildren = gameObject.GetComponentInChildren<Button>();
        if (buttonInChildren)
        {
            buttonInChildren.Select();
            yield break;
        }

        Slider slider = gameObject.GetComponent<Slider>();
        if (slider)
        {
            slider.Select();
            yield break;
        }
    }

    public bool LastValidSelectionExists()
    {
        return lastValidSelection != null && lastValidSelection.activeInHierarchy;
    }

    public void UpdateValidSelection()
    {
        _lastValidSelections = _lastValidSelections
            .Where(s =>
                s.obj != null &&
                s.obj.GetComponent<Selectable>()?.interactable == true &&
                s.obj.GetComponent<Selectable>()?.navigation.mode != Navigation.Mode.None &&
                s.obj.activeInHierarchy)
            .GroupBy(s => s.obj) // prevent duplicates
            .Select(g => g.First())
            .ToList();
    }

    public void SetLastValidSelection()
    {
        var latest = _lastValidSelections
            .Where(s =>
                s.obj != null &&
                s.obj != currentSelectedObj &&
                s.obj.GetComponent<Selectable>()?.navigation.mode != Navigation.Mode.None &&
                s.obj.activeInHierarchy)
            .OrderByDescending(s => s.time)
            .FirstOrDefault();

        lastValidSelection = latest.obj;
    }

    public void ForceLastValidSelection(GameObject gameObject)
    {
        if (gameObject != null)
        {
            lastValidSelection = gameObject;
            _lastValidSelections.Add(new ValidSelection(gameObject, Time.unscaledTime));
        }
    }

    private void LateUpdate()
    {
        if (!InputDeviceHelper.Instance.IsController())
            return;

        GameObject selected = eventsystem.currentSelectedGameObject;

        // Case 1: Selection is invalid or dead
        if (selected != null && (!selected.activeInHierarchy || !selected.GetComponent<Selectable>()?.interactable == true))
        {
            UpdateValidSelection();
            if (LastValidSelectionExists())
            {
                eventsystem.SetSelectedGameObject(lastValidSelection);
            }
            else
            {
                eventsystem.SetSelectedGameObject(null);
            }
        }
        // Case 2: Selection changed
        else if (selected != null && selected != currentSelectedObj)
        {
            currentSelectedObj = selected;

            if (!_lastValidSelections.Any(s => s.obj == currentSelectedObj) &&
                currentSelectedObj.GetComponent<Selectable>()?.interactable == true &&
                currentSelectedObj.activeInHierarchy)
            {
                _lastValidSelections.Add(new ValidSelection(currentSelectedObj, Time.unscaledTime));
            }

            SetLastValidSelection();
        }
        // Case 3: No selection, try to restore
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