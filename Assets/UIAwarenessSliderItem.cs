using UnityEngine;
using UnityEngine.UI;

public class UIAwarenessSliderItem : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public void Setup(int questsCompleted, int maxQuests)
    {
        slider.maxValue = maxQuests;
        slider.value = questsCompleted;
        Destroy(gameObject, 3);
    }
}
