using UnityEngine;

[ExecuteInEditMode]
public class GetMainLightDirection : MonoBehaviour
{
    [SerializeField] private Material skyBoxMaterial;

    private void Update()
    {
        skyBoxMaterial.SetVector("_MainLightDirection", transform.forward);
    }
}