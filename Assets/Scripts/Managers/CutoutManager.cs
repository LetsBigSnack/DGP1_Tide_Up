using UnityEngine;

public class CutoutManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Material[] affectedMaterials;

    private static readonly int PlayerPosID = Shader.PropertyToID("_PlayerPosition");
    private static readonly int CameraPosID = Shader.PropertyToID("_CameraPosition");

    void LateUpdate()
    {
        if (player == null || mainCamera == null) return;

        Vector3 playerPos = player.position;
        Vector3 camPos = mainCamera.transform.position;

        foreach (Material mat in affectedMaterials)
        {
            if (mat == null) continue;
            mat.SetVector(PlayerPosID, playerPos);
            mat.SetVector(CameraPosID, camPos);
        }
        Debug.DrawLine(camPos, playerPos, Color.cyan);
    }

    void OnValidate()
    {
        Debug.Log($"Materials assigned: {affectedMaterials.Length}");
    }
}
