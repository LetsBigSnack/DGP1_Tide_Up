using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Lighting Preset", menuName = "Scriptable Objects/Lighting Preset", order = 1)]

[Serializable]
public class LightingPreset : ScriptableObject
{
    public Gradient AmbientColor;
    public Gradient DirectionalColor;
    public Gradient FogColor;
}