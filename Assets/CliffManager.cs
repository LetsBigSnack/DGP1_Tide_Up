using System;
using System.Collections.Generic;
using UnityEngine;

public class CliffManager : MonoBehaviour
{
    [SerializeField] private int numberOfChecks;
    [SerializeField] private float checkDistance;
    [SerializeField] private float maxHeight;
    [SerializeField] private float maxCheckDistance;
    [SerializeField] private LayerMask layerToCheck;
    private List<GameObject> _cliffCheckers;
    
    private void Awake()
    {
        PopulateChecks();
    }

    private void PopulateChecks()
    {
        float degrees = 360.0f / numberOfChecks;
        float startingAngle = 0.0f;
        
        for (int i = 0; i < numberOfChecks; i++)
        {
            GameObject cliff = Instantiate(new GameObject(), transform);
            cliff.transform.localPosition = new Vector3(0, 0, checkDistance);
            cliff.transform.RotateAround(transform.position, Vector3.up, startingAngle + (i * degrees));
            _cliffCheckers.Add(cliff);
        }
    }

    private void CheckCliffs()
    {
        foreach (GameObject checker in _cliffCheckers)
        {
            RaycastHit hit;
            if (Physics.Raycast(checker.transform.position, Vector3.down, out hit, maxCheckDistance, layerToCheck))
            {
                if (hit.distance < maxHeight)
                {
                    Debug.Log("No Cliff");
                }
                else
                {
                    Debug.Log("Cliff");
                }
            }
            else
            {
                Debug.Log("Cliff");
            }
        }
    }
}
