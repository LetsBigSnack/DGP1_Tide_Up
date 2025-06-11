using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class InteractionData
{
    public string ID;
    public bool hasBeenTriggered;

    public InteractionData(string ID, bool hasBeenTriggered)
    {
        this.ID = ID;
        this.hasBeenTriggered = hasBeenTriggered;
    }
}

public class SystemInteractionManager : MonoBehaviour
{
    public static SystemInteractionManager Instance;

    [SerializeField] private SystemInteractable currentInteractable;
    [SerializeField] private List<InteractionData> datas = new();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<InteractionData> GetDatas()
    {
        return datas;
    }

    public void CreateSystemInteractableData(SystemInteractable sys)
    {
        if(datas.Exists(d => d.ID == sys.GetID()))
        {
            return;
        }

        InteractionData data = new InteractionData(sys.GetID(), sys.HasBeenTriggered());
        datas.Add(data);
    }

    public void SetInteractableData(string ID, bool hasBeenTriggered)
    {
        InteractionData data = datas.Where(d => d.ID == ID).FirstOrDefault();

        if(data == null)
        {
            return;
        }

        data.hasBeenTriggered = hasBeenTriggered;
    }

    public SystemInteractable GetCurrentInteractable()
    {
        return currentInteractable;
    }

    public InteractionData GetDataByID(string ID)
    {
        return datas.Where(d => d.ID == ID).FirstOrDefault();
    }

    public void SetCurrentInteractable(SystemInteractable sys)
    {
        currentInteractable = sys;
    }

    public void CloseSystemInformation()
    {
        if(currentInteractable != null)
        {
            currentInteractable.EndInteraction();
        }
    }
}
