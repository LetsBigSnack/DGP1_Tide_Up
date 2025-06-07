using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

public class UITimeManager : MonoBehaviour
{
    [SerializeField] private GameObject timeChangeContainer;
    [SerializeField] private GameObject timeChangeButton;
    [SerializeField] private Slider timeSlider;
    [SerializeField] TextMeshProUGUI switchTimeText;
    [SerializeField] TextMeshProUGUI timeBeforeSkipText;
    [SerializeField] TextMeshProUGUI timeAfterSkipText;

    [Header("Text Display")]
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI dateText;
    [SerializeField] TextMeshProUGUI weekDayText;
    [SerializeField] TextMeshProUGUI yearText;

    [Header("Wait Screen")]
    [SerializeField] private GameObject waitScreen;
    [SerializeField] private GameObject waitScreenContent;
    [SerializeField] private Slider waitSlider;
    [SerializeField] private float minWaitTime = 1f;
    [SerializeField] private float maxWaitTime = 5f;
    [SerializeField] private Image waitScreenBackground;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float fadeInSpeedMultiplier = 1.8f;
    [SerializeField] private float fadeOutSpeedMultiplier = 2.5f;

    private float _preChangeTime;
    private int _preChangeDay;
    private int _preChangeMonth;
    private int _preChangeYear;
    private int _preChangeWeekCount;
    private JournalType _lastJournalType;

    private int _maxHoursToChange;

    public static UITimeManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        TimeManager.OnTimeChanged += UpdateTimeText;
        TimeManager.OnDayChanged += UpdateDateText;
        TimeManager.OnMonthChanged += UpdateDateText;
        TimeManager.OnYearChanged += UpdateDateText;
        TimeManager.OnYearChanged += UpdateYearText;
        TimeManager.OnWeekDayChanged += UpdateWeekDayText;
        UIJournalManager.OnJournalStateChanged += UpdatePenActive;
    }

    private void OnDisable()
    {
        TimeManager.OnTimeChanged -= UpdateTimeText;
        TimeManager.OnDayChanged -= UpdateDateText;
        TimeManager.OnMonthChanged -= UpdateDateText;
        TimeManager.OnYearChanged -= UpdateDateText;
        TimeManager.OnYearChanged -= UpdateYearText;
        TimeManager.OnWeekDayChanged -= UpdateWeekDayText;
        UIJournalManager.OnJournalStateChanged -= UpdatePenActive;
    }

    public void OpenTimeModal()
    {
        _lastJournalType = UIJournalManager.Instance.GetCurrentState();

        GameStateManager.Instance.SetGameState(GameStateManager.Instance.LastPlayingState);
        UIJournalManager.Instance.SwitchState(JournalType.Closed);

        timeChangeContainer.SetActive(true);
        timeChangeButton.SetActive(false);
        TimeManager.Instance.ToggleTime();
        GameStateManager.Instance.PauseGame();
        GameStateManager.Instance.SetGameState(GameStates.TimeChange);

        _preChangeTime = TimeManager.Instance.CurrentTimeInHours;
        _preChangeDay = TimeManager.Instance.CurrentDay;
        _preChangeMonth = TimeManager.Instance.CurrentSeasonNum;
        _preChangeYear = TimeManager.Instance.CurrentYear;
        _preChangeWeekCount = TimeManager.Instance.CurrentWeekDay;

        timeSlider.value = 0;
        timeBeforeSkipText.text = "Day " + _preChangeDay + ", " + TimeSpan.FromHours(_preChangeTime).ToString(@"hh\:mm");
        timeAfterSkipText.text = "Day " + _preChangeDay + ", " + TimeSpan.FromHours(TimeManager.Instance.CurrentTimeInHours + timeSlider.value).ToString(@"hh\:mm");

        _maxHoursToChange = 0;
    }

    private void UpdatePenActive(JournalType type)
    {
        DeviceType deviceType = InputDeviceHelper.Instance.GetLastDeviceType() == DeviceType.Mouse ? DeviceType.Keyboard : InputDeviceHelper.Instance.GetLastDeviceType();

        if (timeChangeButton.activeInHierarchy && deviceType != DeviceType.Keyboard 
            && type == JournalType.Map
            && !UITideUpBoxManager.Instance.IsOpen
            && UIReUpcycleManager.Instance.GetCurrentState() == ReUpcyclerType.Closed)
        {
            UIEventSystemHelper.Instance.SetFirstSelectedItem(timeChangeButton);
        }
    }

    private void UpdateTimeText(float newTime)
    {
        timeText.text = TimeSpan.FromHours(newTime).ToString(@"hh\:mm");
    }

    private void UpdateDateText(int newDate)
    {
        dateText.text = TimeManager.Instance.GetDate();
    }

    private void UpdateWeekDayText(int newDay)
    {
        if(newDay == 0)
        {
            Debug.Log("Is 0 broski");
            return;
        }
        weekDayText.text = TimeManager.Weekdays[newDay].ToUpper();
    }
    private void UpdateYearText(int newYear)
    {
        yearText.text = TimeManager.Instance.CurrentYear.ToString();
    }

    public void CancelTimeChange()
    {
        TimeManager.Instance.ResetDateAndTime(
            _preChangeTime,
            _preChangeDay,
            _preChangeMonth,
            _preChangeYear,
            _preChangeWeekCount);
        TimeManager.Instance.ToggleTime();
        GameStateManager.Instance.ResumeGame();
        UIJournalManager.Instance.SwitchState(_lastJournalType);
        GameStateManager.Instance.SetGameState(GameStates.InMenu);
        CloseTimeChange();
    }

    public void CloseTimeChange()
    {
        timeChangeContainer.SetActive(false);
        timeChangeButton.SetActive(true);
    }

    public bool IsTimeChangeActive()
    {
        return timeChangeContainer.activeInHierarchy;
    }

    public void SetAsFirstSelectedItem()
    {
        if(InventoryManager.Instance.Items.Count <= 0)
        {
            UIEventSystemHelper.Instance.SetFirstSelectedItem(timeChangeButton);
        }
    }

    public void SubmitTimeChange()
    {
        if(timeSlider.value <= 0)
        {
            CancelTimeChange();
            return;
        }
        TimeManager.Instance.AddTime(timeSlider.value);
        GameStateManager.Instance.ResumeGame();
        GameStateManager.Instance.SetGameState(GameStates.SceneTransition);
        timeChangeContainer.SetActive(false);
        StartCoroutine(WaitTimeSkip());
    }

    private IEnumerator WaitTimeSkip()
    {
        float duration = Mathf.Lerp(minWaitTime, maxWaitTime, _maxHoursToChange / 24f);

        waitScreen.SetActive(true);
        waitScreenContent.SetActive(true);
        waitSlider.value = 0f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);
            waitSlider.value = progress;

            if (waitScreenBackground == null)
            {
                continue;
            }

            Color color = waitScreenBackground.color;

            if (progress < 0.7f)
            {
                color.a = progress * fadeInSpeedMultiplier;
                waitScreenBackground.color = color;
            }

            yield return null;
        }
        TimeManager.Instance.ToggleTime();
        GameStateManager.Instance.ResumeGame();

        yield return StartCoroutine(FadeOutWaitBG());

        waitScreen.SetActive(false);
    }

    private IEnumerator FadeOutWaitBG()
    {
        if (waitScreenBackground == null)
        {
            yield return null;
        }

        waitScreenContent.SetActive(false);

        float elapsed = 0f;
        Color color = waitScreenBackground.color;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            color.a = alpha * fadeOutSpeedMultiplier;
            waitScreenBackground.color = color;
            yield return null;
        }
        timeChangeButton.SetActive(true);
    }

    public void OnSliderChange()
    {
        switchTimeText.text = timeSlider.value.ToString();

        int newDay = _preChangeDay + 1;
        if (_preChangeTime >= 23f && timeSlider.value > 0)
        {
            timeAfterSkipText.text = "Day " + newDay + ", " + TimeSpan.FromHours(TimeManager.Instance.CurrentTimeInHours + timeSlider.value).ToString(@"hh\:mm");
            return;
        }
        if (timeSlider.value + _preChangeTime > 24f)
        {
            timeAfterSkipText.text = "Day " + newDay + ", " + TimeSpan.FromHours(TimeManager.Instance.CurrentTimeInHours + timeSlider.value).ToString(@"hh\:mm");
            return;
        }
        timeAfterSkipText.text = "Day " + _preChangeDay + ", " + TimeSpan.FromHours(TimeManager.Instance.CurrentTimeInHours + timeSlider.value).ToString(@"hh\:mm");
    }
}
