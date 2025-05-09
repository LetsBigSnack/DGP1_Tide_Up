using Data;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum Seasons
{
    Winter = 1,
    Spring = 2,
    Summer = 3,
    Autumn = 4
}

public class UICalendarDescriptionHelper : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI seasonTitle;
    [SerializeField] private TextMeshProUGUI yearNumber;
    [SerializeField] private TextMeshProUGUI weekdayDaynumber;
    [SerializeField] private Seasons currSeason;
    [SerializeField] private GameObject calendarNoteItemPrefab;
    [SerializeField] private GameObject noteItemContainer;

    private int _lastUpdatedDay = 0;
    private List<NpcData> _birthdays = new();

    public static UICalendarDescriptionHelper Instance;
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
        TimeManager.OnDayChanged += UpdateCurrDay;
        TimeManager.OnMonthChanged += UpdateSeasonYear;
    }
    private void OnDisable()
    {
        TimeManager.OnDayChanged -= UpdateCurrDay;
    }

    public void UpdateCurrDay(int newDay)
    {
        if (newDay == _lastUpdatedDay)
        {
            return;
        }
        _lastUpdatedDay = newDay;
        ResetCurrDateNote(); 
    }

    public void ResetCurrDateNote()
    {
        weekdayDaynumber.text = DisplayNoteDate(TimeManager.Instance.GetWeekDay(), TimeManager.Instance.CurrentDay);
        ClearEventContainer();

        List<NpcData> allNpcs = NpcManager.Instance.GetNpcs();

        foreach (NpcData npc in allNpcs)
        {
            if (npc.BirthSeason == TimeManager.Instance.GetCurrentSeason() &&
                npc.BirthDay == TimeManager.Instance.CurrentDay)
            {
                _birthdays.Add(npc);
            }
        }
        SetupBirthdays();
    }

    public void UpdateSeasonYear(int newSeason)
    {
        switch ((Seasons)newSeason)
        {
            case Seasons.Winter:
                currSeason = Seasons.Winter;
                break;
            case Seasons.Spring:
                currSeason = Seasons.Spring;
                break;
            case Seasons.Summer:
                currSeason = Seasons.Summer;
                break;
            case Seasons.Autumn:
                currSeason = Seasons.Autumn;
                break;
        }
        
        seasonTitle.text = currSeason.ToString();
        yearNumber.text = TimeManager.Instance.CurrentYear.ToString();
    }

    public void Setup(string weekDay, int dayNumber, List<NpcData> birthdays)
    {
        weekdayDaynumber.text = DisplayNoteDate(weekDay, dayNumber);

        ClearEventContainer();

        _birthdays = birthdays;
        SetupBirthdays();
    }

    private string DisplayNoteDate(string weekDay, int dayNumber)
    {
        return weekDay + ", Day " + dayNumber;
    }

    public void ClearEventContainer()
    {
        foreach (Transform child in noteItemContainer.transform)
        {
            Destroy(child.gameObject);
        }
        _birthdays = new();
    }

    private void SetupBirthdays()
    {
        foreach (var npc in _birthdays)
        {
            GameObject note = Instantiate(calendarNoteItemPrefab, noteItemContainer.transform);
            note.GetComponent<UICalenderNoteItem>().Setup(
                "Birthday: " + npc.NpcName,
                "All Day",
                npc.HomeDetails,
                npc.Portrait
            );
        }
    }

}
