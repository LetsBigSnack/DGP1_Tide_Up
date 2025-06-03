using UnityEngine;
using Assets.Scripts.Data;
using System.Collections.Generic;

public class UICalendarController : UIJournalSubMenu
{
    [Header("SubMenu")]
    [SerializeField] private GameObject subMenu;

    [Header("UICalenderItems")]
    [SerializeField] private int maxDaysPerMonth = 25;
    [SerializeField] private List<Transform> leftPageRows;
    [SerializeField] private List<Transform> rightPageRows;

    [SerializeField] private GameObject UICurrDayItemPrefab;
    [SerializeField] private GameObject UICurrMonthDayItemPrefab;
    [SerializeField] private GameObject UIOtherMonthDayItemPrefab;

    private int _lastUpdatedDay = 0;

    public static UICalendarController Instance;
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

    public override void CloseMenu()
    {
        subMenu.SetActive(false);
    }

    public override void OpenMenu()
    {
        subMenu.SetActive(true);
        UICalendarDescriptionHelper.Instance.ResetCurrDateNote();
        UpdateCalendar(TimeManager.Instance.CurrentDay - 1);
    }

    private void OnEnable()
    {
        TimeManager.OnDayChanged += UpdateCalendar;
    }
    private void OnDisable()
    {
        TimeManager.OnDayChanged -= UpdateCalendar;
    }

    private void UpdateCalendar(int newDate)
    {
        if (newDate == _lastUpdatedDay)
        {
            return;
        }
        _lastUpdatedDay = newDate;

        ClearCalendar();

        int currentDay = TimeManager.Instance.CurrentDay;

        int totalSlots = leftPageRows.Count * 5 + rightPageRows.Count * 2;

        for (int slot = 0; slot < totalSlots; slot++)
        {
            GameObject prefabToSpawn;
            int displayedDay = slot - (TimeManager.Instance.FirstWeekDayOfMonth - 2);

            if (displayedDay < 1)
            {
                // Previous season overflow
                prefabToSpawn = UIOtherMonthDayItemPrefab;
            }
            else if (displayedDay > maxDaysPerMonth)
            {
                // Next season preview
                prefabToSpawn = UIOtherMonthDayItemPrefab;
            }
            else
            {
                // Current month
                if (displayedDay == currentDay)
                {
                    prefabToSpawn = UICurrDayItemPrefab;
                }
                else
                {
                    prefabToSpawn = UICurrMonthDayItemPrefab;
                }
            }

            GameObject newDayItem = Instantiate(prefabToSpawn, GetParentForSlot(slot));
            newDayItem.GetComponent<UIMonthDayItem>().Setup(displayedDay);
            if (displayedDay == currentDay)
            {
                UIEventSystemHelper.Instance.SetFirstSelectedItem(newDayItem);
                UIMonthDayItem currUIMonthDayItem = newDayItem.GetComponent<UIMonthDayItem>();
                UICalendarDescriptionHelper.Instance?.SetGameObjectAsSelected(currUIMonthDayItem);
            }
        }
    }

    private Transform GetParentForSlot(int slot)
    {
        int week = slot / 7;
        int dayOfWeek = slot % 7;

        if (dayOfWeek < 5)
        {
            return leftPageRows[week];
        }
        else
        {
            return rightPageRows[week];
        }
    }

    private void ClearCalendar()
    {
        foreach (Transform row in leftPageRows)
        {
            foreach (Transform child in row)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (Transform row in rightPageRows)
        {
            foreach (Transform child in row)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
