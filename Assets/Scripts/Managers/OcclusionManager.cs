using System.Collections.Generic;
using UnityEngine;

public class OcclusionManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float radius = 0.5f;

    // Track affected renderers and the count of materials they had set
    private readonly Dictionary<(Renderer renderer, int index), MaterialPropertyBlock> occluded = new();

    void FixedUpdate()
    {
        if (player == null || mainCamera == null) return;

        Vector3 camPos = mainCamera.transform.position;
        Vector3 playerPos = player.position;
        Vector3 dir = playerPos - camPos;
        float distance = dir.magnitude;

        var hits = Physics.SphereCastAll(camPos, radius, dir, distance);
        var currentHits = new HashSet<(Renderer, int)>();

        foreach (var hit in hits)
        {
            var renderer = hit.collider.GetComponent<Renderer>();
            if (renderer == null) continue;

            var materials = renderer.sharedMaterials;

            Debug.Log("All materials hit: " + materials);

            for (int i = 0; i < materials.Length; i++)
            {
                var mat = materials[i];
                if (mat == null || !mat.HasProperty("_FadeAmount")) continue;

                var key = (renderer, i);
                currentHits.Add(key);

                if (!occluded.ContainsKey(key))
                    occluded[key] = new MaterialPropertyBlock();

                var block = occluded[key];
                renderer.GetPropertyBlock(block, i);
                block.SetFloat("_FadeAmount", 0f); // fully transparent
                renderer.SetPropertyBlock(block, i);
            }
        }

        // Reset all previous occluded not hit anymore
        foreach (var key in new List<(Renderer, int)>(occluded.Keys))
        {
            if (!currentHits.Contains(key))
            {
                var block = occluded[key];
                block.SetFloat("_FadeAmount", 1f); // fully visible again
                key.Item1.SetPropertyBlock(block, key.Item2);
                occluded.Remove(key);
            }
        }
    }
}
