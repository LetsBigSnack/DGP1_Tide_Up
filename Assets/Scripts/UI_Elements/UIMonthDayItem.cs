using Data;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMonthDayItem : MonoBehaviour
{
    [SerializeField] private Image charBirthday;
    [SerializeField] private Image eventIcon;
    [SerializeField] private TextMeshProUGUI dayNumber;

    [SerializeField] private int dayNumInSeason;
    [SerializeField] private string weekDay;

    private List<NpcData> _birthdaysOnThisDay = new();

    public void OnClick()
    {
        UICalendarDescriptionHelper.Instance.Setup(weekDay, dayNumInSeason, _birthdaysOnThisDay);
    }

    public void Setup(int number)
    {
        charBirthday.gameObject.SetActive(false);
        eventIcon.gameObject.SetActive(false);

        dayNumInSeason = number;

        Seasons currSeason = TimeManager.Instance.GetCurrentSeason();
        Seasons nextSeason = currSeason + 1;
        Seasons lastSeason = currSeason - 1;

        if (nextSeason == (Seasons)5)
        {
            nextSeason = (Seasons)1;
        }
        if (lastSeason == (Seasons)0)
        {
            lastSeason = (Seasons)4;
        }

        bool isCurrSeason = false;
        bool isNextSeason = false;
        bool isLastSeason = false;

        if (dayNumInSeason <= 0) 
        {
            dayNumInSeason += 25;
            isLastSeason = true;
        }
        else if (dayNumInSeason > 25)
        {
            dayNumInSeason -= 25;
            isNextSeason = true;
        }
        else
        {
            isCurrSeason = true;
        }

        weekDay = TimeManager.Instance.GetWeekDay((TimeManager.Instance.FirstWeekDayOfMonth + number - 2) % 7 + 1);

        dayNumber.text = dayNumInSeason.ToString();

        List<NpcData> allNpcs = NpcManager.Instance?.GetNpcs();

        if(allNpcs == null)
        {
            return;
        }

        foreach (NpcData npc in allNpcs)
        {
            if (npc.BirthSeason == currSeason && isCurrSeason ||
                npc.BirthSeason == nextSeason && isNextSeason ||
                npc.BirthSeason == lastSeason && isLastSeason)
            {
                if (npc.NpcState == NpcStates.Intro)
                {
                    return;
                }
                if (npc.BirthDay == dayNumInSeason)
                {
                    _birthdaysOnThisDay.Add(npc);
                    charBirthday.gameObject.SetActive(true);
                    charBirthday.sprite = npc.Portrait;
                }
            }
        }

        //TODO: add and create events
    }
}
