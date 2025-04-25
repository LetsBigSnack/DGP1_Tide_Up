using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/MapData")]
[Serializable]
public class MapData : ScriptableObject
{
    [SerializeField] private IslandObjectType type;
    [SerializeField] private string title;
    [SerializeField] private Sprite sprite;

    public IslandObjectType Type
    {
        get => type;
    }

    public string Title
    {
        get => title;
    }

    public Sprite Sprite
    {
        get => sprite;
    }
}
