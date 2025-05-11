using System;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniGameManager : MonoBehaviour
{
    
    public static UIMiniGameManager Instance;
    
    [SerializeField] private GameObject uiMiniGameHolder;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private RectTransform sweetSpotMarker;

    [SerializeField] private float indicatorHeight = 2.0f;
    
    private float _totalTime;
    private float _goalTime;
    private float _startTime;
    private bool _isRunning;
    private float _lastGoalPct;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void Initialize(float totalTime, float initialGoalPct, Transform target)
    {
        Show();
        _startTime = Time.time;
        _isRunning = true;
        timeSlider.value = 0;
        timeSlider.maxValue = 1.0f;
        transform.position = target.position + Vector3.up * indicatorHeight;
        
        _lastGoalPct = -1f; // reset state to force placement on first update
        UpdateSlider(0f, initialGoalPct);
    }


    public void UpdateSlider(float progress, float goalPct)
    {
        if (!_isRunning) return;

        timeSlider.value = progress;

        // Only reposition sweet spot if the goal has changed
        if (!Mathf.Approximately(goalPct, _lastGoalPct))
        {
            PositionSweetSpot(goalPct);
            _lastGoalPct = goalPct;
        }
    }
    
    private void PositionSweetSpot(float pct)
    {
        RectTransform sliderRect = timeSlider.GetComponent<RectTransform>();
        float width = sliderRect.rect.width;

        Vector2 pos = sweetSpotMarker.anchoredPosition;
        float markerWidth = sweetSpotMarker.rect.width;

        pos.x = width * pct - (markerWidth * sweetSpotMarker.pivot.x);
        sweetSpotMarker.anchoredPosition = pos;
    }

    public void Show()
    {
        uiMiniGameHolder.SetActive(true);
    }
    
    public void Hide()
    {
        _isRunning = false;
        uiMiniGameHolder.SetActive(false);
    }
}
