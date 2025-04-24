using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMonthDayItem : MonoBehaviour
{
    [SerializeField] private Image charBirthday;
    [SerializeField] private Image eventIcon;
    [SerializeField] private TextMeshProUGUI dayNumber;

    public void OnClick()
    {
        
    }

    public void Setup(int number)
    {
        dayNumber.text = number.ToString();
        // Optionally set birthday or event icon visibility here
    }
}
