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

    [Header("Text Display")]
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI switchTimeText;
    [SerializeField] TextMeshProUGUI dateText;
    [SerializeField] TextMeshProUGUI weekDayText;

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

    private float preChangeTime;
    private int preChangeDay;
    private int preChangeMonth;
    private int preChangeYear;
    private int preChangeWeekCount;

    private int maxHoursToChange;

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
        TimeManager.OnWeekDayChanged += UpdateWeekDayText;
    }

    private void OnDisable()
    {
        TimeManager.OnTimeChanged -= UpdateTimeText;
        TimeManager.OnDayChanged -= UpdateDateText;
        TimeManager.OnMonthChanged -= UpdateDateText;
        TimeManager.OnYearChanged -= UpdateDateText;
        TimeManager.OnWeekDayChanged += UpdateWeekDayText;
    }

    public void OpenTimeModal()
    {
        timeChangeContainer.SetActive(true);
        timeChangeButton.SetActive(false);
        TimeManager.Instance.ToggleTime();
        GameStateManager.Instance.PauseGame();
        GameStateManager.Instance.SetGameState(GameStates.InMenu);

        preChangeTime = TimeManager.Instance.CurrentTimeInHours;
        preChangeDay = TimeManager.Instance.CurrentDay;
        preChangeMonth = TimeManager.Instance.CurrentSeason;
        preChangeYear = TimeManager.Instance.CurrentYear;
        preChangeWeekCount = TimeManager.Instance.CurrentWeekDay;

        switchTimeText.text = TimeSpan.FromHours(TimeManager.Instance.CurrentTimeInHours).ToString(@"hh\:mm");

        maxHoursToChange = 0;
    }

    private void UpdateTimeText(float newTime)
    {
        timeText.text = TimeSpan.FromHours(newTime).ToString(@"hh\:mm");
    }

    private void UpdateDateText(int newDate)
    {
        dateText.text = TimeManager.Instance.getDate();
    }

    private void UpdateWeekDayText(int newDay)
    {
        weekDayText.text = TimeManager.Weekdays[newDay];
    }

    private void UpdateSwitchText()
    {
        switchTimeText.text = TimeSpan.FromHours(TimeManager.Instance.CurrentTimeInHours).ToString(@"hh\:mm");
    }
    public void addTime()
    {
        maxHoursToChange += 1;
        if(maxHoursToChange > 24)
        {
            return;
        }
        TimeManager.Instance.AddTime();
        UpdateSwitchText();
    }

    public void CancelTimeChange()
    {
        TimeManager.Instance.ResetDateAndTime(
            preChangeTime,
            preChangeDay,
            preChangeMonth,
            preChangeYear,
            preChangeWeekCount);
        TimeManager.Instance.ToggleTime();
        GameStateManager.Instance.ResumeGame();
        timeChangeContainer.SetActive(false);
        timeChangeButton.SetActive(true);
    }

    public void SubmitTimeChange()
    {
        GameStateManager.Instance.ResumeGame();
        GameStateManager.Instance.SetGameState(GameStates.SceneTransition);
        timeChangeContainer.SetActive(false);
        StartCoroutine(WaitTimeSkip());
    }

    private IEnumerator WaitTimeSkip()
    {
        float duration = Mathf.Lerp(minWaitTime, maxWaitTime, maxHoursToChange / 24f);

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
}
