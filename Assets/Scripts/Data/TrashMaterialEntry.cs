using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TrashMaterialEntry
{
    [SerializeField]
    private TrashMaterialData _trashMaterialData;
    
    [SerializeField]
    private int _amount;

    public TrashMaterialEntry(TrashMaterialData material, int amount)
    {
        this._trashMaterialData = material;
        this._amount = amount;
    }
    public TrashMaterialData TrashMaterialData
    {
        get { return _trashMaterialData; }
        set { _trashMaterialData = value; }
    }
    public int Amount
    {
        get { return _amount; }
        set { _amount = value; }
    }
}
