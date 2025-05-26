using System.Collections.Generic;
using UnityEngine;

public class BuildSpotData
{
    public string buildSpotID;
    public bool isUnlocked;

    public BuildSpotData(string id, bool isUnlocked)
    {
        this.buildSpotID = id;
        this.isUnlocked = isUnlocked;
    }
}

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    private List<BuildSpotData> _buildSpots = new List<BuildSpotData>();

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

        _buildSpots = LoadBuildSpot();
    }

    public void FetchData(BuildSpotInteractable spot)
    {
        if (!_buildSpots.Exists(s => s.buildSpotID == spot.GetID()))
        {
            BuildSpotData newData = new BuildSpotData(spot.GetID(), spot.IsUnlocked());
            _buildSpots.Add(newData);
            spot.Setup(newData);
        }
        else
        {
            BuildSpotData loadedSpot = _buildSpots.Find(s => s.buildSpotID == spot.GetID());
            spot.Setup(loadedSpot);
        }
    }

    public List<BuildSpotData> LoadBuildSpot()
    {
        //logic to fetch all saved buildspots
        return new List<BuildSpotData>();
    }

    public void SaveBuildSpots() 
    {
        //logic for saving
    }
}
