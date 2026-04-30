using UnityEngine;
using UnityEngine.Playables;

public class BackgroundSequenceTrigger : MonoBehaviour
{
    [Header("Timeline")]
    [SerializeField] private PlayableDirector director;

    [Header("Background Asset")]
    [SerializeField] private BackgroundFollower backgroundAsset;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        enabled = false;

        if (backgroundAsset != null)
        {
            Rigidbody2D playerRb = other.GetComponentInParent<Rigidbody2D>();
            backgroundAsset.Initialize(playerRb);
        }

        if (director != null)
            director.Play();

        Debug.Log("Sequence triggered");
    }
}
