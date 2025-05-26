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

    [SerializeField] private string description;

    public override InteractableType Type => InteractableType.Build;

    public override void Interact()
    {
        if(EnvironmentManager.Instance.GetCurrentIsland().State >= state)
        {
            UIBuildManager.Instance.Setup(null, description, trashNeeded, this);
        }
        else
        {
            UI_ToastManager.Instance.SpawnToastMessage(ToastType.Important,"To rebuild this spot you need to raise your awareness level first! Keep cleaning!");
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
        if (data == null)
        {
            return false;
        }
        return data.isUnlocked;
    }
    public bool Build()
    {
        bool canBuild = true;

        foreach (TrashMaterialEntry t in trashNeeded)
        {
            if (!canBuild)
            {
                break;
            }
            Debug.LogWarning("DEKI");
            int neededAmount = t.Amount;
            int currentAmount = InventoryManager.Instance.GetMaterialAmount(t.TrashMaterialData.type);
            if (currentAmount < neededAmount)
            {
                canBuild = false;
            }
        }

        if (canBuild)
        {
            foreach (TrashMaterialEntry t in trashNeeded)
            {
                InventoryManager.Instance.RemoveMaterial(t.TrashMaterialData.type, t.Amount);
            }
            activeObj.SetActive(true);
            data.isUnlocked = true;
            gameObject.SetActive(false);
        }
        return canBuild;
    }

    public override void ShowInteractability(bool show)
    {
        highlight = show;
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (ID == null || ID.Length <= 0)
        {
            //double check
            List<BuildSpotInteractable> gameObjects = new List<BuildSpotInteractable>();
            ID = GenerateID();
        }
    }
    #endif


    private string GenerateID()
    {
        List<BuildSpotInteractable> gameObjects = new List<BuildSpotInteractable>();
        string tempID = Guid.NewGuid().ToString();

        if (gameObjects.Exists(c => c.ID == tempID))
        {
            return GenerateID();
        }
        return tempID;
        
    }
}
