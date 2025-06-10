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
    [SerializeField] private float boatUpScale = 1;

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
            
            if(representation != null)
            {
                ShowIndicator(representation, target);
            }
        }
        else
        {
            HideIndicator();
        }
    }

    private void ShowIndicator(UIInteractionRepresentation interactRepresentation, GameObject target)
    {
        if (_activeIndicator == null)
        {
            GameObject indicator = Instantiate(indicatorPrefab, worldspaceCanvas.transform);
            _activeIndicator = indicator.GetComponent<UIInteractIndicator>();
        }
        else
        {
            _activeIndicator.transform.localScale = Vector3.one;
        }

        if (interactRepresentation == null)
        {
            throw new System.NullReferenceException();
        }
            
        _activeIndicator.SetUpIndicator(interactRepresentation, target);
        if (GameStateManager.Instance.GetGameState() == GameStates.PlayingBoat)
        {
            float scale = Mathf.Clamp(boatUpScale, 0.1f, 10f);
            _activeIndicator.transform.localScale *= scale;
        }
    }

    private void HideIndicator()
    {
        if (_activeIndicator != null)
            _activeIndicator.Hide();
    }
}
