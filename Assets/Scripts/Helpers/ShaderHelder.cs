using System;
using UnityEngine;
using UnityEngine.Rendering;


[ExecuteAlways]
public class ShaderHelder : MonoBehaviour
{
    private void Update()
    {

        GlobalKeyword shaderActive = GlobalKeyword.Create("_SHADERHELPER");
        
        if (Application.isPlaying)
        {
            Shader.SetKeyword(shaderActive, true);
        }
        else
        {
            Shader.SetKeyword(shaderActive, false);
        }
        
    }
}
