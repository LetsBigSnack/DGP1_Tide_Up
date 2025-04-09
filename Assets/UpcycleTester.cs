using UnityEngine;
using Data;

public class UpcycleTester : MonoBehaviour
{
    public void AddMaterial(TrashMaterialType type)
    {
        var data = DataUtil.Instance.GetMaterialByType(type);
        bool success = UpcycleManager.Instance.AddIngredient(data);
        Debug.Log(success ? $"Added {type} to upcycler." : $"Failed to add {type}.");
    }

    public void Upcycle()
    {
        bool success = UpcycleManager.Instance.Upcycle();
        Debug.Log(success ? "Upcycle successful!" : "Upcycle failed.");
    }
    
    public void RemoveAll()
    {
        bool success = UpcycleManager.Instance.RemoveAllIngredients();
        Debug.Log(success ? "Upcycle removed all!" : "Upcycle failed.");
    }
}