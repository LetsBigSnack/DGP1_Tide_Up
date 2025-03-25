using UnityEngine;
using TMPro;
using System;

public class UI_TimeManager : MonoBehaviour
{
    [Header("Text Display")]
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI switchTimeText;
    [SerializeField] TextMeshProUGUI dateText;

    private float preChangeTime;
    private int preChangeDay;
    private int preChangeMonth;
    private int preChangeYear;

    private int maxHoursToChange;

    private void OnEnable()
    {
        TimeManager.OnTimeChanged += UpdateTimeText;
        TimeManager.OnDayChanged += UpdateDateText;
        TimeManager.OnMonthChanged += UpdateDateText;
        TimeManager.OnYearChanged += UpdateDateText;
    }

    private void OnDisable()
    {
        TimeManager.OnTimeChanged -= UpdateTimeText;
        TimeManager.OnDayChanged -= UpdateDateText;
        TimeManager.OnMonthChanged -= UpdateDateText;
        TimeManager.OnYearChanged -= UpdateDateText;
    }

    public void OpenTimeModal()
    {
        TimeManager.Instance.ToggleTime();

        preChangeTime = TimeManager.Instance.CurrentTimeInHours;
        preChangeDay = TimeManager.Instance.CurrentDay;
        preChangeMonth = TimeManager.Instance.CurrentMonth;
        preChangeYear = TimeManager.Instance.CurrentYear;

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
            preChangeYear);
        TimeManager.Instance.ToggleTime();
    }

    public void SubmitTimeChange()
    {
        TimeManager.Instance.ToggleTime();
    }
}
