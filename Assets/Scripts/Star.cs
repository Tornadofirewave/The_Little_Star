using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.collider.GetComponentInParent<InteractButton>()?.TryActivate(ActivatorType.Star);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponentInParent<InteractButton>()?.TryActivate(ActivatorType.Star);
        Destroy(gameObject);
    }
}
