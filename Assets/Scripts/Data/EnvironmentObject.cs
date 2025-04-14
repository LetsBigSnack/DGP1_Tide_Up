using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentVisual
{
    [SerializeField] private EnvironmentState state;
    [SerializeField] private Material material;
    [SerializeField] private Mesh mesh;
    
    public EnvironmentState State
    {
        get => state;
        set => state = value;
    }

    public Material Material
    {
        get => material;
        set => material = value;
    }

    public Mesh Mesh
    {
        get => mesh;
        set => mesh = value;
    }

   
}


public class EnvironmentObject : MonoBehaviour
{
    [SerializeField] private int islandID;
    [SerializeField] private List<EnvironmentVisual> visuals = new List<EnvironmentVisual>();
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    
    private void Awake()
    {
        EnvironmentManager.OnEnvironmentStateChanged += OnChangeVisualRepresentation;
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    
    private void OnDestroy()
    {
        EnvironmentManager.OnEnvironmentStateChanged -= OnChangeVisualRepresentation;
    }

    
    private void StartVisual()
    {
        EnvironmentState state = EnvironmentManager.Instance.GetStateOfIsland(islandID);
        ChangeVisualRepresentation(state);
    }
    
    void Start()
    {
         StartVisual();
    }
    private void OnChangeVisualRepresentation(EnvironmentState state, int id)
    {
        if (id != islandID)
        {
            return;
        }
        ChangeVisualRepresentation(state);
    }

    private void ChangeVisualRepresentation(EnvironmentState state)
    {
        EnvironmentVisual visual = visuals.Find(v => v.State == state);

        if (visual == null)
        {
            throw new NullReferenceException();
        }
        
        _meshFilter.mesh = visual.Mesh;
        _meshRenderer.material = visual.Material;
    }

    public void AssignIsland(int islandID)
    {
        this.islandID = islandID;
    }
}
