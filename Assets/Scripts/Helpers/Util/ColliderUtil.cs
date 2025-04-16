using UnityEngine;

namespace Helpers.Util
{
    public class ColliderUtil
    {
        public static float GetColliderHeight(GameObject gameObject)
        {
            
            Collider collider = gameObject.GetComponent<Collider>();

            if (collider == null)
            {
                return 0f;
            }
            
            switch (collider)
            {
                case BoxCollider box:
                    return box.size.y * box.transform.localScale.y;
                case SphereCollider sphere:
                    return sphere.radius * 2f * sphere.transform.localScale.y;
                case CapsuleCollider capsule:
                    return capsule.height * capsule.transform.localScale.y;
                case MeshCollider meshCollider:
                {
                    Bounds bounds = meshCollider.sharedMesh.bounds;
                    Vector3 scaledSize = Vector3.Scale(bounds.size, meshCollider.transform.localScale);
                    return scaledSize.y;
                }
                default:
                    Debug.LogWarning("Collider type not supported.");
                    return 0f;
            }
        }
        
        public static Vector3 GetTopPosition(GameObject gameObject)
        {
            Collider collider = gameObject.GetComponent<Collider>();
            if (collider == null) return gameObject.transform.position;

            Bounds bounds = collider.bounds;
            return new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
        }
    }
}