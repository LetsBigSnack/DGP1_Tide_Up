using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [SerializeField] private Light mainLight;

    [SerializeField, Range(0,24)] private float currentTimeInHours;
    [SerializeField, Range(0, 24)] private float newTimeInHours;

    [SerializeField] private float minutesPerDay;

    [SerializeField] private int currentDay = 1;
    [SerializeField] private int currentMonth = 1;
    [SerializeField] private int currentYear = 2025;

    private Dictionary<int, int> months = new Dictionary<int, int>();

    [Header("LightColorPresets")]
    [SerializeField] private Gradient skyColor;
    [SerializeField] private Gradient equatorColor;
    [SerializeField] private Gradient sunColor;

    //testing purpose only.
    [SerializeField] bool isTimePaused;
    [SerializeField] TextMeshProUGUI timeText;

    public float CurrentTimeInHours
    {
        get { return currentTimeInHours; }
        set { currentTimeInHours = value; }
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
    }

    private void Update()
    {
        if (!isTimePaused)
        {
            currentTimeInHours += Time.deltaTime * (24 / (minutesPerDay * 60));
        }

        UpdateDate();
        UpdateMainLightRotation();
        UpdateLight();
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
        if (currentTimeInHours > 24)
        {
            if(currentDay + 1 > months[currentMonth])
            {
                currentDay = 1;
                currentMonth += 1;
                if(currentMonth + 1 > 12)
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

    //testing purpose only will be put in a ui manager

    public void OpenTimeModal()
    {
        ToggleTime();
        newTimeInHours = currentTimeInHours;
        UpdateTimeText();
    }

    private void UpdateTimeText()
    {
        timeText.text = newTimeInHours.ToString("F0") + ":00";
    }

    public void addTime()
    {
        if(newTimeInHours + 1  > 24)
        {
            newTimeInHours = 1;
            UpdateTimeText();
            return;
        }
        newTimeInHours += 1;
        UpdateTimeText();
    }

    /* GOING BACK IN TIME NOT POSSIBLE!!
    public void subTractTime()
    {
        if (newTimeInHours - 1 < 0)
        {
            newTimeInHours = 23;
            UpdateTimeText();
            return;
        }
        newTimeInHours -= 1;
        UpdateTimeText();
    }
    */

    public void ToggleTime()
    {
        isTimePaused = !isTimePaused;
    }

    public void SetNewTime()
    {


        currentTimeInHours = newTimeInHours;
        ToggleTime();
    }
}
