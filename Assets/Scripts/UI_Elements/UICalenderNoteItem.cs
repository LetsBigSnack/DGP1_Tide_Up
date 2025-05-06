using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICalenderNoteItem : MonoBehaviour
{
    [SerializeField] private Image EventIcon;
    [SerializeField] private TextMeshProUGUI eventTitle;
    [SerializeField] private TextMeshProUGUI eventTime;
    [SerializeField] private TextMeshProUGUI eventLocation;

    public void Setup(string title, string time, string location, Sprite icon)
    {
        eventTitle.text = title;
        eventTime.text = time;
        eventLocation.text = location;


        if (EventIcon == null)
        {
            EventIcon.gameObject.SetActive(false);
            return;
        }

        EventIcon.sprite = icon;
        EventIcon.gameObject.SetActive(true);
    }
}
