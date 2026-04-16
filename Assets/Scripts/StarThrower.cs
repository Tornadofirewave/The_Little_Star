using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class StarThrower : MonoBehaviour
{
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private float throwSpeedX = 8f;
    [SerializeField] private float throwSpeedY = 10f;
    [SerializeField] private Vector2 spawnOffset = new Vector2(0.5f, 0.5f);
    [SerializeField] private float throwCooldown = 0.5f;

    private int facingDirection = 1;
    private float cooldownTimer = 0f;
    private readonly List<Collider2D> activeStars = new List<Collider2D>();

    private void Update()
    {
        if (Variables.Object(gameObject).Get<bool>("InDialogue")) return;

        float h = Input.GetAxis("Horizontal");
        if (h != 0f)
            facingDirection = h > 0f ? 1 : -1;

        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.X) && cooldownTimer <= 0f)
            ThrowStar();
    }

    private void ThrowStar()
    {
        Vector3 spawnPos = transform.position + new Vector3(spawnOffset.x * facingDirection, spawnOffset.y, 0f);
        GameObject star = Instantiate(starPrefab, spawnPos, Quaternion.identity);
        Collider2D starCol = star.GetComponent<Collider2D>();

        star.GetComponent<Rigidbody2D>().velocity = new Vector2(throwSpeedX * facingDirection, throwSpeedY);
        Physics2D.IgnoreCollision(starCol, GetComponent<Collider2D>());

        activeStars.RemoveAll(c => c == null);
        foreach (Collider2D other in activeStars)
            Physics2D.IgnoreCollision(starCol, other);

        activeStars.Add(starCol);
        cooldownTimer = throwCooldown;
    }
}
