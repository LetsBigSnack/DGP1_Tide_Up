using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("Day Duration")]
    [SerializeField] private float minutesPerDay;

    [Header("CurrentTime/Date")]
    [SerializeField] private float currentTimeInHours;
    [SerializeField] private int currentDay = 1;
    [SerializeField] private int currentMonth = 1;
    [SerializeField] private int currentYear = 2025;
    private Dictionary<int, int> months = new Dictionary<int, int>();

    public static Action<float> OnTimeChanged;
    public static Action<int> OnDayChanged;
    public static Action<int> OnMonthChanged;
    public static Action<int> OnYearChanged;

    [Header("LightSource")]
    [SerializeField] private Light mainLight;

    [Header("Light- & ColorPresets")]
    [SerializeField] private Gradient skyColor;
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

    public int CurrentMonth
    {
        get { return currentMonth; }
        set { currentMonth = value; }
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
            Destroy(this);
            return;
        }

        Instance = this;
        SetCalender();
        UpdateDate();
    }

    public string getTime()
    {
        return TimeSpan.FromHours(currentTimeInHours).ToString(@"hh\:mm");
    }

    public string getDate()
    {
        string date = currentDay + " / " + currentMonth + " / " + currentYear;
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
            UpdateMainLightRotation();
            UpdateLight();
        }
    }

    private void SetCalender()
    {
        months.Add(1, 31);
        months.Add(2, 28);
        months.Add(3, 31);
        months.Add(4, 30);
        months.Add(5, 31);
        months.Add(6, 30);
        months.Add(7, 31);
        months.Add(8, 31);
        months.Add(9, 30);
        months.Add(10, 31);
        months.Add(11, 30);
        months.Add(12, 31);
    }

    private void UpdateDate()
    {
        if (currentTimeInHours >= 24)
        {
            CheckIfLeapYear();
            if(currentDay + 1 > months[currentMonth])
            {
                currentDay = 1;
                currentMonth += 1;

                if (currentMonth + 1 > 12)
                {
                    currentMonth = 1;
                    currentYear += 1;
                }
            }
            else
            {
                currentDay += 1;
            }
            currentTimeInHours = 0;
        }

        if (!isTimePaused)
        {
            OnYearChanged?.Invoke(currentMonth);
            OnYearChanged?.Invoke(currentYear);
            OnDayChanged?.Invoke(currentDay);
            OnTimeChanged?.Invoke(currentTimeInHours);
        }
    }

    private void CheckIfLeapYear()
    {
        if(currentMonth == 2 && currentYear % 4 == 0)
        {
            months[2] = 29;
            Debug.Log("Its a leap year");
            return;
        }
        months[2] = 28;
    }
 
    private void UpdateMainLightRotation()
    {
        float mainLightRotation = Mathf.Lerp(-90, 270, currentTimeInHours / 24);
        mainLight.transform.rotation = Quaternion.Euler(mainLightRotation, mainLight.transform.rotation.y, mainLight.transform.rotation.z);
    }

    private void UpdateLight()
    {
        float timeFraction = currentTimeInHours / 24;
        //https://docs.unity3d.com/6000.0/Documentation/ScriptReference/RenderSettings-ambientEquatorColor.html
        //https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Gradient.Evaluate.html
        RenderSettings.ambientEquatorColor = equatorColor.Evaluate(timeFraction);
        RenderSettings.ambientSkyColor = skyColor.Evaluate(timeFraction);
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

    public void ResetDateAndTime(float time, int day, int month, int year)
    {
        currentTimeInHours = time;
        currentDay = day;
        currentMonth = month;
        currentYear = year;
    }
}
