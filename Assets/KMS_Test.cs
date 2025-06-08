using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[Serializable]
public class SAN_WO_HUAN
{
    public GameObject gameObject;
    public double timeStamp;

    public SAN_WO_HUAN(GameObject obj)
    {
        gameObject = obj;
        timeStamp = DateTime.Now.ToOADate();
    } 
}


public class KMS_Test : EventSystem
{
    public List<SAN_WO_HUAN> lastValidSelection;
     public static KMS_Test Instance;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        
        lastValidSelection = new List<SAN_WO_HUAN>();
    }

    //TODO: Find a way to fucking fix this fucking piece of shit.
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetSelectedGameObject(GameObject selected, BaseEventData pointer)
    {
        base.SetSelectedGameObject(selected, pointer);

        if (selected != null && lastValidSelection.All(x => x.gameObject != selected))
        {
            lastValidSelection.Add(new SAN_WO_HUAN(selected));
        }
    }
    
    
    public  void SetSelectedGameObject(GameObject selected)
    {
        base.SetSelectedGameObject(selected);

        if (selected != null && lastValidSelection.All(x => x.gameObject != selected))
        {
            lastValidSelection.Add(new SAN_WO_HUAN(selected));
        }
    }
    
    protected void FixedUpdate()
    {
        base.Update();
        lastValidSelection = lastValidSelection.Where(c=> c != null && c.gameObject != null && c.gameObject.GetComponentInChildren<Selectable>().interactable).ToList();
        if (currentSelectedGameObject == null && lastValidSelection.Count > 0)
        {
            SetSelectedGameObject(lastValidSelection.OrderByDescending(c=>c.timeStamp).Select(c=>c.gameObject).First());
        }
    }


    public void ClearSHIT()
    {
        lastValidSelection.Clear();
    }
}

