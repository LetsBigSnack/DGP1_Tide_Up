using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
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
    [SerializeField] private int elapsedDays = 0;
    [SerializeField] private float currentTimeInHours;
    [SerializeField] private int currentDay = 1;
    [SerializeField] private int currentSeason = 1;
    [SerializeField] private int currentYear = 1;
    [SerializeField] private int currentWeekDayCount = 1;
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

    public int CurrentSeason
    {
        get { return currentSeason; }
        set { currentSeason = value; }
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

    public string getTime()
    {
        return TimeSpan.FromHours(currentTimeInHours).ToString(@"hh\:mm");
    }

    public string getDate()
    {
        string date = currentDay + " / " + currentSeason + " / " + currentYear;
        if (currentDay < 10)
        {
            return "0" + date;
        }
        return date;
    }

    private void Update()
    {
        if (!isTimePaused)
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
                if (currentSeason + 1 > maxSeasons)
                {
                    currentSeason = 1;
                    currentYear += 1;
                }
                else
                {
                    currentSeason += 1;
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
            OnMonthChanged?.Invoke(currentSeason);
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

    public void AddTime()
    {
        currentTimeInHours += 1f;
        UpdateDate();
    }

    public void ResetDateAndTime(float time, int day, int month, int year, int weekDayCount)
    {
        currentTimeInHours = time;
        currentDay = day;
        currentSeason = month;
        currentYear = year;
        currentWeekDayCount = weekDayCount;
    }
}
