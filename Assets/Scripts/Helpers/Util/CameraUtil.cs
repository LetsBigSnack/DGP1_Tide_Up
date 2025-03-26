using UnityEngine;

public class CameraUtil
{
    public static bool IsVisibleToCamera(Vector3 targetPosition)
    {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(targetPosition);
        return viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;
    }

    public static bool HasLineOfSight(Vector3 targetPosition)
    {
        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = (targetPosition - origin).normalized;
        float distance = Vector3.Distance(origin, targetPosition);

        return !Physics.Raycast(origin, direction, distance);
    }
}
