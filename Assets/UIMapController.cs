using Assets.Scripts.Data;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMapController : UIJournalSubMenu
{
    //TODO: Islands should know what is on them (houses, shops, etc)
    //Islands should know where their CenterPoint is for the camera

    public static UIMapController Instance;

    [Header("SubMenu")]
    [SerializeField] private GameObject mapMenu;

    [Header("Island Name")]
    [SerializeField] private TextMeshProUGUI islandName;

    [Header("MapCamera")]
    [SerializeField] private GameObject mapCamera;

    [Header("Legend Setup")]
    [SerializeField] private GameObject legendItemPrefab;
    [SerializeField] private Transform legendItemParent;
    [SerializeField] private List<MapData> mapPossibilities = new List<MapData>();

    private List<GameObject> _currentMapLegendItems = new List<GameObject>();

    private List<GameObject> _placeNames = new List<GameObject>();

    public override void CloseMenu()
    {
        mapMenu.SetActive(false);
    }

    public override void OpenMenu()
    {
        SetMapCamera();
        mapMenu.SetActive(true);
        UpdateMap();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPlaceName(GameObject gameObject)
    {
        if (!_placeNames.Contains(gameObject))
        {
            _placeNames.Add(gameObject);
        }
    }

    public void ToggleVisiblePlaces()
    {
        if (_placeNames.Count <= 0)
        {
            return;
        }

        foreach (GameObject o in _placeNames)
        {
            if(o == null)
            {
                continue;
            }
            o.SetActive(!o.activeInHierarchy);
        }
        _placeNames.Clear();
    }

    public void ActivatePlaceNames()
    {
        if(_placeNames.Count <= 0)
        {
            return;
        }
        foreach(GameObject o in _placeNames)
        {
            if (o == null)
            {
                continue;
            }
            o.SetActive(true);
        }
    }

    private void SetName()
    {
        islandName.text = EnvironmentManager.Instance.GetCurrentIsland().IslandName;
    }

    private void UpdateMap()
    {
        ClearMapLegend();
        SetName();
        ActivatePlaceNames();

        foreach (IslandObjectType o in EnvironmentManager.Instance.GetCurrentIsland().ObjectsOnIsland)
        {
            MapData data = mapPossibilities.Find(m => m.Type == o);
            GameObject legendItem = Instantiate(legendItemPrefab, legendItemParent);
            legendItem.GetComponent<UIMapLegendItem>().Setup(data);
            _currentMapLegendItems.Add(legendItem);
        }
    }

    private void ClearMapLegend()
    {
        if(_currentMapLegendItems.Count <= 0)
        {
            return;
        }

        foreach(GameObject o in _currentMapLegendItems)
        {
            Destroy(o);
        }

        _currentMapLegendItems.Clear();
    }

    private void SetMapCamera()
    {
        Vector3 islandCenter = EnvironmentManager.Instance.GetCurrentIsland().IslandCenter;

        mapCamera.transform.position = new Vector3(islandCenter.x, mapCamera.transform.position.y, islandCenter.z);
    }
}
