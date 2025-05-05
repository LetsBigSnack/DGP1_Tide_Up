using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class EnvironmentObject : MonoBehaviour
{
    [SerializeField] private IslandObjectType type;
    [SerializeField] private int islandID;
    [SerializeField] private List<EnvironmentVisual> visuals = new List<EnvironmentVisual>();
    [SerializeField] private GameObject currentActiveGameObject;
    [SerializeField] private EnvironmentState state;

    public IslandObjectType Type
    {
        get => type;
        set => type = value;
    }
    
    private void Awake()
    {
        EnvironmentManager.OnEnvironmentStateChanged += OnChangeVisualRepresentation;
    }
    
    private void OnDestroy()
    {
        EnvironmentManager.OnEnvironmentStateChanged -= OnChangeVisualRepresentation;
    }

    
    private void StartVisual()
    {
        if (type != IslandObjectType.Other && type != IslandObjectType.House)
        {
            return;
        }
        EnvironmentState state = EnvironmentManager.Instance.GetStateOfIsland(islandID);
        ChangeVisualRepresentation(state);
    }
    
    void Start()
    {
         StartVisual();
    }
    private void OnChangeVisualRepresentation(EnvironmentState state, int id)
    {
        if (type != IslandObjectType.Other && type != IslandObjectType.House)
        {
            return;
        }

        if (id != islandID)
        {
            return;
        }
        ChangeVisualRepresentation(state);
    }

    private void ChangeVisualRepresentation(EnvironmentState state)
    {
        //TODO REMOVE AFTER MILESTONE PLANNING!
        if(visuals.Count <= 0)
        {
            return;
        }

        if(currentActiveGameObject != null && this.state != state)
        {
            currentActiveGameObject.SetActive(false);
        }

        EnvironmentVisual visual = visuals.Find(v => v.State == state);

        visual.EnvironmentObj.SetActive(true);

        currentActiveGameObject = visual.EnvironmentObj;
    }

    public void AssignIsland(int islandID)
    {
        this.islandID = islandID;
    }
}
