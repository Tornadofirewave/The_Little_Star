using UnityEngine;

public class StarAbilityPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        StarThrower thrower = other.GetComponent<StarThrower>();
        if (thrower == null) return;

        thrower.canThrow = true;
        Destroy(gameObject);
    }
}
