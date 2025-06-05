using System.Collections.Generic;
using UnityEngine;

public class CutoutManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask occluderLayer;

    [Header("Shader Property Names")]
    [SerializeField] private string playerPositionProperty = "PlayerPosition";
    [SerializeField] private string cameraPositionProperty = "CameraPosition";

    private static int PlayerPosID;
    private static int CameraPosID;

    private readonly HashSet<Renderer> currentlyAffected = new();

    void Awake()
    {
        PlayerPosID = Shader.PropertyToID(playerPositionProperty);
        CameraPosID = Shader.PropertyToID(cameraPositionProperty);
    }

    void LateUpdate()
    {
        if (player == null || mainCamera == null) return;

        Vector3 playerPos = player.position;
        Vector3 camPos = mainCamera.transform.position;
        Vector3 direction = playerPos - camPos;
        float distance = direction.magnitude;

        // Cast all objects between camera and player
        RaycastHit[] hits = Physics.RaycastAll(camPos, direction, distance, occluderLayer);
        HashSet<Renderer> newlyHit = new();

        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer == null) continue;

            Material[] materials = renderer.materials;

            foreach (Material mat in materials)
            {
                if (mat.HasProperty(PlayerPosID))
                {
                    mat.SetVector(PlayerPosID, playerPos);
                    mat.SetVector(CameraPosID, camPos);
                }
            }

            newlyHit.Add(renderer);
        }

        // Reset old ones that are no longer hit
        foreach (Renderer r in currentlyAffected)
        {
            if (!newlyHit.Contains(r))
            {
                Material[] materials = r.materials;

                foreach (Material mat in materials)
                {
                    if (mat.HasProperty(PlayerPosID))
                    {
                        mat.SetVector(PlayerPosID, new Vector3(9999, 9999, 9999));
                        mat.SetVector(CameraPosID, new Vector3(9999, 9999, 9999));
                    }
                }
            }
        }

        currentlyAffected.Clear();
        foreach (Renderer r in newlyHit)
        {
            currentlyAffected.Add(r);
        }

        Debug.DrawLine(camPos, playerPos, Color.cyan);
    }
}
