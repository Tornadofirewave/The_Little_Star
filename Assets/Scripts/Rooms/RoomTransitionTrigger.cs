using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RoomTransitionTrigger : MonoBehaviour
{
    public static event System.Action OnPlayerCrossed;

    [SerializeField] private Room parentRoom;

    private void Reset()
    {
        parentRoom = GetComponentInParent<Room>();
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var rb = other.GetComponent<Rigidbody2D>();
        if (rb == null || rb.velocity.y <= 0f) return;

        OnPlayerCrossed?.Invoke();
        RoomManager.Instance.AdvanceFrom(parentRoom);
    }
}
