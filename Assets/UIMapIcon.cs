using UnityEngine;

public class UIMapIcon : MonoBehaviour
{
    private void LateUpdate()
    {
        this.gameObject.transform.rotation = new Quaternion(transform.rotation.x, 0, 0, transform.rotation.w);
    }
}
