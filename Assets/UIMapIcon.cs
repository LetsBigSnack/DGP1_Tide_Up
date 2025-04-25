using UnityEngine;

public class UIMapIcon : MonoBehaviour
{
    [SerializeField] private bool isStatic = true;
    [SerializeField] private bool isPlaceName = false;

    private void Start()
    {
        if (isPlaceName)
        {
            UIMapController.Instance.AddPlaceName(this.gameObject);
        }
    }

    private void LateUpdate()
    {
        if (!isStatic)
        {
            this.gameObject.transform.rotation = new Quaternion(transform.rotation.x, 0, 0, transform.rotation.w);
        }
    }
}
