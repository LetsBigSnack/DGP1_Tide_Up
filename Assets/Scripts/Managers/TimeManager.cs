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
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log("OnSceneLoaded - TIME");
        mainLight = GameObject.FindGameObjectWithTag("Sun")?.GetComponent<Light>();
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
        currentSeasonNum = month;
        currentYear = year;
        currentWeekDayCount = weekDayCount;
    }
}
