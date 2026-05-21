using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class StarThrower : MonoBehaviour
{
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private float forwardThrowSpeedX = 8f;
    [SerializeField] private float forwardThrowSpeedY = 4f;
    [SerializeField] private float upwardThrowSpeedX = 2.5f;
    [SerializeField] private float upwardThrowSpeedY = 10f;
    [SerializeField] private float downwardThrowSpeedX = 1.5f;
    [SerializeField] private float downwardThrowSpeedY = -11f;
    [SerializeField] private Vector2 spawnOffset = new Vector2(0.5f, 0.5f);
    [SerializeField] private float throwCooldown = 0.5f;

    [HideInInspector] public bool canThrow = false;

    private const float DirectionDeadZone = 0.5f;

    private int facingDirection = 1;
    private float cooldownTimer = 0f;
    private readonly List<Collider2D> activeStars = new List<Collider2D>();
    private Collider2D playerCollider;

    private enum ThrowDirection
    {
        Forward,
        Up,
        Down
    }

    private void Awake()
    {
        playerCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        var objVars = Variables.Object(gameObject);
        if (objVars.IsDefined("InDialogue") && objVars.Get<bool>("InDialogue")) return;
        if (!canThrow) return;

        float h = Input.GetAxisRaw("Horizontal");
        if (h != 0f)
            facingDirection = h > 0f ? 1 : -1;

        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.X) && cooldownTimer <= 0f)
            ThrowStar();
    }

    private void ThrowStar()
    {
        ThrowDirection throwDirection = GetThrowDirection();
        Vector2 launchVelocity = GetLaunchVelocity(throwDirection);
        Vector2 spawnDirection = launchVelocity.sqrMagnitude > 0f ? launchVelocity.normalized : Vector2.right * facingDirection;
        Vector3 spawnPos = transform.position + new Vector3(
            spawnOffset.x * spawnDirection.x,
            spawnOffset.y + Mathf.Max(0f, spawnDirection.y * spawnOffset.y),
            0f);

        GameObject star = Instantiate(starPrefab, spawnPos, Quaternion.identity);
        Collider2D starCol = star.GetComponent<Collider2D>();
        Star starComponent = star.GetComponent<Star>();
        Rigidbody2D starRb = star.GetComponent<Rigidbody2D>();

        if (starRb != null)
            starRb.velocity = launchVelocity;

        if (starCol != null && playerCollider != null)
            Physics2D.IgnoreCollision(starCol, playerCollider);

        activeStars.RemoveAll(c => c == null);
        if (starCol != null)
        {
            foreach (Collider2D other in activeStars)
                Physics2D.IgnoreCollision(starCol, other);
        }

        if (starCol != null)
            activeStars.Add(starCol);

        cooldownTimer = throwCooldown;
    }

    private ThrowDirection GetThrowDirection()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        if (vertical >= DirectionDeadZone)
            return ThrowDirection.Up;

        if (vertical <= -DirectionDeadZone)
            return ThrowDirection.Down;

        return ThrowDirection.Forward;
    }

    private Vector2 GetLaunchVelocity(ThrowDirection throwDirection)
    {
        return throwDirection switch
        {
            ThrowDirection.Up => new Vector2(upwardThrowSpeedX * facingDirection, upwardThrowSpeedY),
            ThrowDirection.Down => new Vector2(downwardThrowSpeedX * facingDirection, downwardThrowSpeedY),
            _ => new Vector2(forwardThrowSpeedX * facingDirection, forwardThrowSpeedY)
        };
    }
}
