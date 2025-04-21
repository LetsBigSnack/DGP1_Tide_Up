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

    [Header("LightSource")]
    [SerializeField] private Light mainLight;

    [Header("Light- & ColorPresets")]
    [SerializeField] private Gradient ambientColor;
    [SerializeField] private Gradient fogColor;
    [SerializeField] private Gradient equatorColor;
    [SerializeField] private Gradient sunColor;

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

    
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateDate();
    }

    

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        //
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
            currentTimeInHours += Time.deltaTime * (24 / (minutesPerDay * 60));
            UpdateDate();
            if (mainLight != null)
            {
                UpdateMainLightRotation();
                UpdateLight();
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
 
    private void UpdateMainLightRotation()
    {
        mainLight.transform.rotation = Quaternion.Euler(new Vector3(((currentTimeInHours/24)*360-90f), -30, 0));
    }

    private void UpdateLight()
    {
        float timeFraction = currentTimeInHours / 24;
        //https://docs.unity3d.com/6000.0/Documentation/ScriptReference/RenderSettings-ambientEquatorColor.html
        //https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Gradient.Evaluate.html
        RenderSettings.fogColor = fogColor.Evaluate(timeFraction);
        RenderSettings.ambientEquatorColor = equatorColor.Evaluate(timeFraction);
        RenderSettings.ambientSkyColor = ambientColor.Evaluate(timeFraction);
        mainLight.color = sunColor.Evaluate(timeFraction);
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
