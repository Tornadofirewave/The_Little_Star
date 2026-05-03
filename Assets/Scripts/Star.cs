using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private string impactSfxId = "star_impact";
    [SerializeField] private GameObject impactParticlePrefab;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) return;
        collision.collider.GetComponentInParent<InteractButton>()?.TryActivate(ActivatorType.Star);
        SpawnImpact(transform.position);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;
        other.GetComponentInParent<InteractButton>()?.TryActivate(ActivatorType.Star);
        SpawnImpact(transform.position);
        Destroy(gameObject);
    }

    private void SpawnImpact(Vector3 pos)
    {
        AudioManager.Instance?.Play(impactSfxId);
        CameraShake.Instance?.Shake();
        if (impactParticlePrefab != null)
        {
            GameObject fx = Instantiate(impactParticlePrefab, pos, Quaternion.identity);
            Destroy(fx, 2f);
        }
    }
}
