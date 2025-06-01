using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class SmoothCameraMove : MonoBehaviour
{
    [Header("Points to Move Between")]
    public GameObject pointA;
    
    public GameObject pointB;

    [Header("Timing & Curve")]
    public float duration = 2f;

    [Tooltip("AnimationCurve to ease the move: X = normalized time (0→1), Y = normalized interpolation (0→1).")]
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private bool _movingTowardsB = true;

    private Coroutine _moveRoutine;

    public void Reset()
    {
        if (pointA != null)
        {
            transform.position = pointA.gameObject.transform.position;
            transform.rotation = pointA.gameObject.transform.rotation;
            _movingTowardsB = true;
        }
    }

    private void Awake()
    {
        if (pointA != null)
        {
            transform.position = pointA.gameObject.transform.position;
            transform.rotation = pointA.gameObject.transform.rotation;
            _movingTowardsB = true;
        }
    }
    
    public void TriggerMove()
    {
        if (_moveRoutine != null)
            StopCoroutine(_moveRoutine);

        _moveRoutine = StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        float elapsed = 0f;
        
        Vector3 fromPos = _movingTowardsB && pointA != null ? pointA.gameObject.transform.position : pointB.gameObject.transform.position;
        Vector3 toPos   = _movingTowardsB && pointB != null ? pointB.gameObject.transform.position : pointA.gameObject.transform.position;

        Quaternion fromRot = _movingTowardsB && pointA != null ? pointA.gameObject.transform.rotation : pointB.gameObject.transform.rotation;
        Quaternion toRot   = _movingTowardsB && pointB != null ? pointB.gameObject.transform.rotation : pointA.gameObject.transform.rotation;
        
        transform.position = fromPos;
        transform.rotation = fromRot;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            
            float curveVal = moveCurve.Evaluate(t);

            transform.position = Vector3.Lerp(fromPos, toPos, curveVal);
            transform.rotation = Quaternion.Slerp(fromRot, toRot, curveVal);

            yield return null;
        }
        
        transform.position = toPos;
        transform.rotation = toRot;
        
        _moveRoutine = null;
    }
}
