using UnityEngine;
using UnityEngine.UI;

public class UIQuestIndicatorItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private GameObject target;
    [SerializeField] private float padding;
    [SerializeField] private Material instanceMaterial;
    [SerializeField] private float raycastRadius = 5f; // Radius for detecting Player
    [SerializeField] private LayerMask detectionLayer; // Layer to specify which objects to detect (Player layer)

    private void LateUpdate()
    {
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            bool isPlayerNearby = CheckPlayerNearby();

            Color color = image.color;
            color.a = (distance <= raycastRadius && isPlayerNearby) ? 1 : 0;
            image.color = color;

            transform.position = new Vector3(
                target.transform.position.x,
                target.transform.position.y + padding,
                target.transform.position.z
            );
        }
    }

    private bool CheckPlayerNearby()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, raycastRadius, detectionLayer);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    public void Setup(GameObject target, Sprite sprite, float padding)
    {
        this.target = target;
        this.image.sprite = sprite;
        this.padding = padding;
    }
}