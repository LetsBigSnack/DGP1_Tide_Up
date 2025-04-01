using UnityEngine;
using static UnityEditor.Progress;

public class RecyclerManager : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 5f;
    [SerializeField] private Color gizmoColor = new Color(1f, 0.7f, 0.2f, 0.2f);

    private bool _playerInRange = false;


    public static RecyclerManager Instance;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Recycle()
    {
        if (!_playerInRange) return;

        ItemData firstItem = InventoryManager.Instance.TestTrashItem();
        bool removedItem = InventoryManager.Instance.TryRemoveItem(firstItem);

        if(removedItem)
        {
            foreach (TrashMaterialData mat in firstItem.materials)
            {
                InventoryManager.Instance.AddMaterial(mat, 1);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            _playerInRange = false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
