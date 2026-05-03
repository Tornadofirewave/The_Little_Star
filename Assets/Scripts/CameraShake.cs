using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    public Vector3 Offset { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private float defaultDuration = 0.15f;
    [SerializeField] private float defaultMagnitude = 0.12f;

    public void Shake(float duration = -1f, float magnitude = -1f)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(
            duration < 0f ? defaultDuration : duration,
            magnitude < 0f ? defaultMagnitude : magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = 1f - elapsed / duration;
            float x = (Mathf.PerlinNoise(elapsed * 40f, 0f) - 0.5f) * 2f * magnitude * t;
            float y = (Mathf.PerlinNoise(0f, elapsed * 40f) - 0.5f) * 2f * magnitude * t;
            Offset = new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Offset = Vector3.zero;
    }
}
