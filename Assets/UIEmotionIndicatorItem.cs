using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIEmotionIndicatorItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float padding;
    [SerializeField] private float destroyTime;
    [SerializeField] private float raycastRadius = 5f;
    [SerializeField] private LayerMask detectionLayer;
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        StartCoroutine(PlayEmotionEnd());
    }

    public void Setup(GameObject target, Sprite sprite, float padding)
    {
        this.image.sprite = sprite;
        this.padding = padding;
        this.transform.position = new Vector3(
            target.transform.position.x,
            target.transform.position.y + padding,
            target.transform.position.z);
    }

    private void EndEmotion()
    {
        _anim.Play("emotion_End");
    }

    public void DestroyEmotion()
    {
        Destroy(gameObject);
    }

    private IEnumerator PlayEmotionEnd()
    {
        yield return new WaitForSeconds(destroyTime);
        EndEmotion();
    }
}