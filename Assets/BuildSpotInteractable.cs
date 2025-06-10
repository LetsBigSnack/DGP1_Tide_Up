using System;
using Data;
using System.Collections.Generic;
using UnityEngine;

public class BuildSpotInteractable : Interactable
{
    [SerializeField] private string ID;
    [SerializeField] private EnvironmentState state;
    [SerializeField] private bool highlight;
    [SerializeField] private List<TrashMaterialEntry> trashNeeded;

    [SerializeField] private BuildSpotData data;

    [SerializeField] private GameObject activeObj;

    [TextArea(0,20)]
    [SerializeField] private string description;
    [TextArea(0,20)]
    [SerializeField] private string subtext;

    [SerializeField] private Sprite sprite;

    public override InteractableType Type => InteractableType.Build;

    public override void Interact()
    {
        if (GameStateManager.Instance.GetGameState() != GameStates.PlayingCharacter || GameStateManager.Instance.GetGameState() != GameStates.Building)
        {

            if (EnvironmentManager.Instance.GetCurrentIsland().State >= state)
            {
                UIBuildManager.Instance.Setup(sprite, description, subtext, trashNeeded, this);
            }
            else
            {
                UI_ToastManager.Instance.SpawnToastMessage(
                    ToastType.Important,
                    "To rebuild this spot you need to raise your awareness level first! Keep cleaning!"
                );
            }
        }
    }

    private void Start()
    {
        BuildManager.Instance.FetchData(this);
    }

    public void Setup(BuildSpotData data)
    {
        this.data = data;

        if (data.isUnlocked)
        {
            activeObj.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public BuildSpotData GetData()
    {
        return data;
    }

    public string GetID()
    {
        return ID;
    }

    public bool IsUnlocked()
    {
        return data != null && data.isUnlocked;
    }

    public bool Build()
    {
        foreach (TrashMaterialEntry t in trashNeeded)
        {
            int neededAmount = t.Amount;
            int currentAmount = InventoryManager.Instance.GetMaterialAmount(t.TrashMaterialData.type);
            if (currentAmount < neededAmount)
            {
                return false;
            }
        }

        foreach (TrashMaterialEntry t in trashNeeded)
        {
            InventoryManager.Instance.RemoveMaterial(t.TrashMaterialData.type, t.Amount);
        }

        activeObj.SetActive(true);
        data.isUnlocked = true;
        gameObject.SetActive(false);
        return true;
    }

    public override void ShowInteractability(bool show)
    {
        highlight = show;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(ID))
        {
            ID = GenerateID();
        }
    }
#endif

    private string GenerateID()
    {
        return Guid.NewGuid().ToString();
    }
}
