using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 desired = target.position + offset;
        Vector3 shake = CameraShake.Instance != null ? CameraShake.Instance.Offset : Vector3.zero;
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime) + shake;
    }
}
