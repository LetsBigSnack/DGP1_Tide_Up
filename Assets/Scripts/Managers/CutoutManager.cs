using UnityEngine;

public class CutoutManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;

    private static readonly int PlayerPosID = Shader.PropertyToID("_PlayerPosition");
    private static readonly int CameraPosID = Shader.PropertyToID("_CameraPosition");

    void FixedUpdate()
    {
        if (player == null || mainCamera == null) return;

        Vector3 playerPos = player.position;
        Vector3 camPos = mainCamera.transform.position;

        Shader.SetGlobalVector(PlayerPosID, playerPos);
        Shader.SetGlobalVector(CameraPosID, camPos);

        Debug.DrawLine(camPos, playerPos, Color.cyan);

        Debug.Log("Its happening, player is at: " + playerPos);
    }
}
