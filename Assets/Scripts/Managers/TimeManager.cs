using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("Day Duration")]
    [SerializeField] private float minutesPerDay;

    [Header("MaxSettings")]
    [SerializeField] private int maxSeasons;
    [SerializeField] private int maxDays;

    [Header("CurrentTime/Date")] 
    [SerializeField] private int elapsedDays = 1;
    [SerializeField] private float currentTimeInHours;
    [SerializeField] private int currentDay = 1;
    [SerializeField] private int currentSeasonNum = 1;
    [SerializeField] private int currentYear = 1;
    [SerializeField] private int currentWeekDayCount = 1;
    [SerializeField] private int firstWeekDayOfMonth = 1;
    [SerializeField] private string currentWeekDay = "";
    
    public static Dictionary<int, string> Weekdays = new Dictionary<int, string>(){
        { 1,"Monday" },{ 2,"Tuesday" },{ 3,"Wednesday" },{ 4,"Thursday" },{ 5,"Friday" },{ 6,"Saturday" },{ 7,"Sunday" }};
    
    
    public static Action<float> OnTimeChanged;
    public static Action<int> OnDayChanged;
    public static Action<int> OnMonthChanged;
    public static Action<int> OnYearChanged;
    public static Action<int> OnWeekDayChanged;

    [SerializeField] private Light directionalLight;
    [SerializeField] private LightingPreset preset;




    [Header("Testing Values")]
    //testing purpose only.
    [SerializeField] bool isTimePaused;

    public float CurrentTimeInHours
    {
        get { return currentTimeInHours; }
        set { currentTimeInHours = value; }
    }

    public int CurrentDay
    {
        get { return currentDay; }
        set { currentDay = value; }
    }

    public int CurrentSeasonNum
    {
        get { return currentSeasonNum; }
        set { currentSeasonNum = value; }
    }

    public int CurrentYear
    {
        get { return currentYear; }
        set { currentYear = value; }
    }

    public int CurrentWeekDay
    {
        get { return currentWeekDayCount; }
        set { currentWeekDayCount = value; }
    }
    public int FirstWeekDayOfMonth
    {
        get { return firstWeekDayOfMonth; }
        set { firstWeekDayOfMonth = value; }
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateDate();
    }

    

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        OnTimeChanged = null;
        OnDayChanged= null;
        OnMonthChanged= null;
        OnYearChanged= null;
        OnWeekDayChanged= null;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log("OnSceneLoaded - TIME");
        directionalLight = GameObject.FindGameObjectWithTag("Sun")?.GetComponent<Light>();
        if(directionalLight != null)
        {
            RenderSettings.sun = directionalLight;
        }
    }

    public string GetTime()
    {
        return TimeSpan.FromHours(currentTimeInHours).ToString(@"hh\:mm");
    }

    public string GetDate()
    {
        string date = currentDay + " / " + currentSeasonNum + " / " + currentYear;
        if (currentDay < 10)
        {
            return "0" + date;
        }
        return date;
    }

    public string GetWeekDay(int? day = null)
    {
        if (day == null)
        {
            Debug.Log("CurrWeekDayCOunt == " + currentWeekDayCount);
            return Weekdays[currentWeekDayCount];
        }

        return Weekdays[(int)day];
    }

    public Seasons GetCurrentSeason()
    {
        return (Seasons)currentSeasonNum;
    }

    private void Update()
    {
        if (!isTimePaused && GameStateManager.Instance.GetGameState() != GameStates.SceneTransition)
        {
            float baseTimeSpeed = (24f / (minutesPerDay * 60f));
            currentTimeInHours += Time.deltaTime * baseTimeSpeed;
            UpdateDate();
            if (directionalLight != null)
            {
                UpdateLighting((currentTimeInHours / 24f));
            }
        }
    }

    public void UpdateWeekDay()
    {
        currentWeekDayCount += 1;
        if(currentWeekDayCount > 7)
        {
            currentWeekDayCount = 1;
        }
        currentWeekDay = Weekdays[currentWeekDayCount];
    }

    private void UpdateDate()
    {
        if (currentTimeInHours >= 24)
        {
            elapsedDays += 1;
            if(currentDay + 1 > maxDays)
            {
                currentDay = 1;
                UpdateWeekDay();

                firstWeekDayOfMonth = currentWeekDayCount;

                if (currentSeasonNum + 1 > maxSeasons)
                {
                    currentSeasonNum = 1;
                    currentYear += 1;
                }
                else
                {
                    currentSeasonNum += 1;
                }
            }
            else
            {
                currentDay += 1;
                UpdateWeekDay();
            }
            currentTimeInHours = 0;
        }

        if (!isTimePaused)
        {
            OnTimeChanged?.Invoke(currentTimeInHours);
            OnDayChanged?.Invoke(currentDay);
            OnMonthChanged?.Invoke(currentSeasonNum);
            OnYearChanged?.Invoke(currentYear);
            OnWeekDayChanged?.Invoke(currentWeekDayCount);
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.FogColor.Evaluate(timePercent);

        if (directionalLight != null)
        {
            directionalLight.color = preset.DirectionalColor.Evaluate(timePercent);

            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }

    }

    public void ToggleTime()
    {
        isTimePaused = !isTimePaused;
    }

    public void AddTime(float timeToAdd)
    {
        for (int i = 0; i < timeToAdd; i++)
        {
            currentTimeInHours += 1f;
            UpdateDate();
        }
    }

    public void ResetDateAndTime(float time, int day, int month, int year, int weekDayCount)
    {
        currentTimeInHours = time;
        currentDay = day;
        currentSeasonNum = month;
        currentYear = year;
        currentWeekDayCount = weekDayCount;
    }
}
