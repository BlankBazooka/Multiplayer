using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private Transform target;
    public Vector3 offset = new Vector3(0f, 0.8f, 0.2f);

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void LateUpdate()
    {
        if (target == null) return;
        transform.position = target.TransformPoint(offset);
        transform.rotation = target.rotation;
    }
}