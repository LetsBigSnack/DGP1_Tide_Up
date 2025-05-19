using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIMiniGameManager : MonoBehaviour
{
    
    public static UIMiniGameManager Instance;
    
    [SerializeField] private GameObject uiMiniGameHolder;
    [SerializeField] private Slider horSlider; 
    [SerializeField] private RectTransform horSweetSpotMarker;
    [SerializeField] private Slider vertSlider; 
    [SerializeField] private RectTransform vertSweetSpotMarker;
    [SerializeField] private float indicatorHeight = 2.0f;

    [SerializeField] private Sprite hand;
    [SerializeField] private Sprite claw;
    [SerializeField] private Sprite shovel;
    [SerializeField] private Sprite trash;
    [SerializeField] private Sprite star;

    [SerializeField] private Image handle;
    [SerializeField] private Image horIndicatorImage;

    [SerializeField] private GameObject fishingButtons;
    [SerializeField] private GameObject digPickButton;


    private bool _isRunning;
    private float _horLastGoalPct;
    private float _verLastGoalPct;

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
    
    public void Initialize(float totalTime, float initialGoalPct, Transform target, MiniGameType gameType = MiniGameType.PickUp, float verTime = 0, float vertGoalPct = 0)
    {
        Show(gameType);
        _isRunning = true;
        horSlider.value = 0;
        horSlider.maxValue = 1.0f;
        transform.position = target.position + Vector3.up * indicatorHeight;
        
        _horLastGoalPct = -1f; // reset state to force placement on first update
        UpdateHorSlider(0f, initialGoalPct);
        
        
        if (gameType == MiniGameType.Fishing)
        {
            handle.sprite = claw;
            horIndicatorImage.sprite = trash;
            fishingButtons.SetActive(true);
            vertSlider.value = 0;
            vertSlider.maxValue = 1.0f;
            _verLastGoalPct = -1f; // reset state to force placement on first update
            UpdateVerSlider(0f, initialGoalPct);
        }

        if(gameType == MiniGameType.PickUp)
        {
            handle.sprite = hand;
            horIndicatorImage.sprite = star;
            digPickButton.SetActive(true);
        }

        if(gameType == MiniGameType.Digging)
        {
            handle.sprite = shovel;
            horIndicatorImage.sprite = star;
            digPickButton.SetActive(true);
        }
        
    }


    public void UpdateHorSlider(float progress, float goalPct)
    {
        if (!_isRunning) return;

        horSlider.value = progress;

        if (!Mathf.Approximately(goalPct, _horLastGoalPct))
        {
            HorPositionSweetSpot(goalPct);
            _horLastGoalPct = goalPct;
        }
    }
    
    public void UpdateVerSlider(float progress, float goalPct)
    {
        if (!_isRunning) return;

        vertSlider.value = progress;

        if (!Mathf.Approximately(goalPct, _verLastGoalPct))
        {
            VertPositionSweetSpot(goalPct);
            _verLastGoalPct = goalPct;
        }
    }
    
    private void HorPositionSweetSpot(float pct)
    {
        RectTransform sliderRect = horSlider.GetComponent<RectTransform>();
        float width = sliderRect.rect.width;

        Vector2 pos = horSweetSpotMarker.anchoredPosition;
        float markerWidth = horSweetSpotMarker.rect.width;

        pos.x = width * pct - (markerWidth * horSweetSpotMarker.pivot.x);
        horSweetSpotMarker.anchoredPosition = pos;
    }
    
    private void VertPositionSweetSpot(float pct)
    {
        RectTransform sliderRect = vertSlider.GetComponent<RectTransform>();
        float width = sliderRect.rect.width;

        Vector2 pos = vertSweetSpotMarker.anchoredPosition;
        float markerWidth = vertSweetSpotMarker.rect.width;

        pos.x = width * pct - (markerWidth * vertSweetSpotMarker.pivot.x);
        vertSweetSpotMarker.anchoredPosition = pos;
    }

    public void Show(MiniGameType gameType)
    {
        uiMiniGameHolder.SetActive(true);

        if (gameType == MiniGameType.Fishing)
        {
            vertSlider.gameObject.SetActive(true);
            vertSweetSpotMarker.gameObject.SetActive(true);
        }
        
    }
    
    public void Hide()
    {
        _isRunning = false;
        uiMiniGameHolder.SetActive(false);
        vertSlider.gameObject.SetActive(false);
        vertSweetSpotMarker.gameObject.SetActive(false);
        fishingButtons.SetActive(false);
        digPickButton.SetActive(false);
    }
}
