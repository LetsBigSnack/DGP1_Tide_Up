using System;
using System.Collections.Generic;
using UnityEngine;

public class OceanManager : MonoBehaviour
{
    
    public static OceanManager Instance;
    
    [SerializeField]
    private List<OceanPreset> oceanPresets;
    
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

    public OceanPreset GetOceanPreset(OceanType oceanType)
    {

        OceanPreset oceanPreset = oceanPresets.Find((o) => o.type == oceanType);

        if (oceanPreset == null)
        {
            throw new NullReferenceException();
        }
        
        return oceanPreset;
    }
    
}
