using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoatMiniGame : MonoBehaviour
{
    [SerializeField] private Slider horSlider; 
    [SerializeField] private GameObject rhythmPrefab;
    
    [SerializeField] private Slider progressSlider;
    [SerializeField] private RectTransform progressSweetSpotMarker;
    
    private List<GameObject> rhythms = new List<GameObject>();
    
    private void PlaceIndicators(List<MiniGameNote> buttons)
    {
        foreach (MiniGameNote note in buttons)
        {
            GameObject go = Instantiate(rhythmPrefab, horSlider.transform.transform);
            UIRhytmButton rhytmButton = go.GetComponentInChildren<UIRhytmButton>();
            RhytmSweetSpot(note.percentage, go);
            rhytmButton.Setup(note);
            rhythms.Add(go);
        }

    }

    private void RemoveIndicators()
    {
        foreach (GameObject go in rhythms)
        {
            Destroy(go);
        }
        rhythms.Clear();
    }
    
    private void ProgressPositionSweetSpot(float pct)
    {
        RectTransform sliderRect = progressSlider.GetComponent<RectTransform>();
        float width = sliderRect.rect.width;

        Vector2 pos = progressSweetSpotMarker.anchoredPosition;
        float markerWidth = progressSweetSpotMarker.rect.width;

        pos.x = width * pct - (markerWidth * progressSweetSpotMarker.pivot.x);
        progressSweetSpotMarker.anchoredPosition = pos;
    }
    
    private void RhytmSweetSpot(float pct , GameObject rhytmPrefab)
    {
        RectTransform sliderRect = horSlider.GetComponent<RectTransform>();
        float width = sliderRect.rect.width;

        RectTransform rhytm = rhytmPrefab.GetComponent<RectTransform>();
        Vector2 pos = rhytm.anchoredPosition;
        float markerWidth = rhytm.rect.width;

        pos.x = width * pct - (markerWidth * rhytm.pivot.x);
        rhytm.anchoredPosition = pos;
    }
    
    

    public void UpdateSlider(float time, float percentage, float threshold, List<MiniGameNote> buttons)
    {
        RemoveIndicators();
        horSlider.value = time;
        progressSlider.value = percentage;
        ProgressPositionSweetSpot(threshold);
        PlaceIndicators(buttons);
        
        Debug.Log("Threshold" + threshold);
    }

    
    public void Initialize(List<MiniGameNote> buttons)
    {
        horSlider.value = 0;
        horSlider.maxValue = 1.0f;
        progressSlider.value = 0.0f;
        progressSlider.maxValue = 1.0f;
        RemoveIndicators();
        PlaceIndicators(buttons);
    }
}
