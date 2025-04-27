using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "OceanPreset", menuName = "Ocean/Ocean Preset", order = 1)]
public class OceanPreset : ScriptableObject
{
    public OceanType type;
    public List<Octave> octaves = new List<Octave>();
}

public enum OceanType
{
    Calm,
    Stormy,
    Gentle,
    Custom
}