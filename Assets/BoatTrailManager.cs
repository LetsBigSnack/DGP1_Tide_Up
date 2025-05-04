using UnityEngine;

public class BoatTrailManager : MonoBehaviour
{
    public RenderTexture trailTexture;
    public Material scrollFadeMaterial;
    public Material waterMaterial;
    public Transform boat;

    public float textureWorldSize = 20f; // width of the RT in world units

    private Vector3 lastBoatPos;

    void Start()
    {
        lastBoatPos = boat.position;

        // Assign trail texture to materials
        scrollFadeMaterial.SetTexture("_MainTex", trailTexture);
        waterMaterial.SetTexture("_TrailTex", trailTexture);
    }

    void Update()
    {
        if (scrollFadeMaterial.shader.name != "Custom/TrailScrollFade")
        {
            Debug.LogError("ScrollFadeMaterial shader is incorrect: " + scrollFadeMaterial.shader.name);
        }
        
        float scrollMultiplier = 100f;

        Vector3 currentPos = boat.position;
        Vector3 deltaWorld = currentPos - lastBoatPos;
        lastBoatPos = currentPos;

        Debug.Log(currentPos);
        Debug.Log(deltaWorld);

        // Convert world delta to UV offset
        float uvOffsetX = -deltaWorld.x / textureWorldSize * scrollMultiplier;
        float uvOffsetY = -deltaWorld.z / textureWorldSize * scrollMultiplier;

        scrollFadeMaterial.SetFloat("_UVOffsetX", uvOffsetX);
        scrollFadeMaterial.SetFloat("_UVOffsetY", uvOffsetY);

        Debug.Log("UV offset: (" + uvOffsetX + ", " + uvOffsetY + ")");
        Debug.Log("UV offset (from material): (" +
                  scrollFadeMaterial.GetFloat("_UVOffsetX") + ", " +
                  scrollFadeMaterial.GetFloat("_UVOffsetY") + ")");

        // ✅ Create a temporary buffer
        RenderTexture temp = RenderTexture.GetTemporary(trailTexture.width, trailTexture.height, 0, trailTexture.format);

        Debug.Log("Material used in Blit: " + scrollFadeMaterial.shader.name);
    
        // ✅ Perform scroll/fade from current trail into temp
        scrollFadeMaterial.SetTexture("_MainTex", trailTexture);
        Graphics.Blit(trailTexture, temp, scrollFadeMaterial);

        // ✅ Prevent reuse of trailTexture during write-back
        scrollFadeMaterial.SetTexture("_MainTex", null);

        // ✅ Copy temp back into the original trail texture
        Graphics.Blit(temp, trailTexture);
        RenderTexture.ReleaseTemporary(temp);

        // ✅ Optional: pass boat position to water shader
        waterMaterial.SetVector("_BoatPosition", boat.position);
    }

}