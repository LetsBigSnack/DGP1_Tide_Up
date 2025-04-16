using System;
using Data;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[Serializable]
public class UIInteractionRepresentation
{
    [SerializeField] private InteractableType type;
    [SerializeField] private string text;
    [SerializeField] private Sprite sprite;
    [SerializeField] private float padding;

    public float Padding
    {
        get => padding;
        set => padding = value;
    }

    public InteractableType Type
    {
        get => type;
        set => type = value;
    }

    public string Text
    {
        get => text;
        set => text = value;
    }

    public Sprite Sprite
    {
        get => sprite;
        set => sprite = value;
    }
}


public class UIInteractionIndicationManager : MonoBehaviour
{
    [SerializeField] private Canvas worldspaceCanvas;
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private List<UIInteractionRepresentation> representations;
    
    private UIInteractIndicator _activeIndicator;
    
    private void OnEnable()
    {
        InteractionManager.OnInteractionChanged += HandleInteractionChange;
    }

    private void OnDisable()
    {
        InteractionManager.OnInteractionChanged -= HandleInteractionChange;
    }

    private void HandleInteractionChange(bool isActive, InteractableType? type, GameObject target)
    {
        if (isActive && target != null)
        {
            
            UIInteractionRepresentation representation = representations.Find(x => x.Type == type);
            
            ShowIndicator(representation, target);
        }
        else
        {
            HideIndicator();
        }
    }

    //For testing there is a text and the type gets converted to it.
    //TODO: Make it so correct img will be shown depending on type
    private void ShowIndicator(UIInteractionRepresentation interactRepresentation, GameObject target)
    {
        if (_activeIndicator == null)
        {
            GameObject indicator = Instantiate(indicatorPrefab, worldspaceCanvas.transform);
            _activeIndicator = indicator.GetComponent<UIInteractIndicator>();

        }

        if (interactRepresentation == null)
        {
            throw new System.NullReferenceException();
        }
            
        _activeIndicator.SetUpIndicator(interactRepresentation, target);
    }

    private void HideIndicator()
    {
        if (_activeIndicator != null)
            _activeIndicator.Hide();
    }

}
