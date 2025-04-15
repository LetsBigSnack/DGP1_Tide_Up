using Data;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPickUpManager : MonoBehaviour
{
    [SerializeField] private Canvas worldspaceCanvas;
    [SerializeField] private GameObject indicatorPrefab;

    [SerializeField] private float amountAboveInteractable = 2f;

    private GameObject activeIndicator;

    private void OnEnable()
    {
        InteractionManager.OnInteractionChanged += HandleInteractionChange;
    }

    private void OnDisable()
    {
        InteractionManager.OnInteractionChanged -= HandleInteractionChange;
    }

    private void HandleInteractionChange(bool isActive, InteractableType? type, Transform targetTransform)
    {
        if (isActive && targetTransform != null)
        {
            ShowIndicator(type.ToString(), targetTransform);
        }
        else
        {
            HideIndicator();
        }
    }

    //For testing there is a text and the type gets converted to it.
    //TODO: Make it so correct img will be shown depending on type
    private void ShowIndicator(string label, Transform target)
    {
        if (activeIndicator == null)
            activeIndicator = Instantiate(indicatorPrefab, worldspaceCanvas.transform);

        activeIndicator.SetActive(true);

        var text = activeIndicator.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
            text.text = label;

        activeIndicator.transform.position = target.position + Vector3.up * amountAboveInteractable;
        activeIndicator.transform.rotation = Quaternion.LookRotation(activeIndicator.transform.position - Camera.main.transform.position);
    }

    private void HideIndicator()
    {
        if (activeIndicator != null)
            activeIndicator.SetActive(false);
    }

}
