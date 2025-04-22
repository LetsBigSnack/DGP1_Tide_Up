using UnityEngine;
using TMPro;
using System;

public class UITimeManager : MonoBehaviour
{
    [Header("Text Display")]
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI switchTimeText;
    [SerializeField] TextMeshProUGUI dateText;
    [SerializeField] TextMeshProUGUI weekDayText;

    private float preChangeTime;
    private int preChangeDay;
    private int preChangeMonth;
    private int preChangeYear;
    private int preChangeWeekCount;

    private int maxHoursToChange;

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
        TimeManager.Instance.ToggleTime();
        GameStateManager.Instance.PauseGame();

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
    }

    public void SubmitTimeChange()
    {
        TimeManager.Instance.ToggleTime();
        GameStateManager.Instance.ResumeGame();
    }
}
