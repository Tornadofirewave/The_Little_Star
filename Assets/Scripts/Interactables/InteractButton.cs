using UnityEngine;
using UnityEngine.Events;

public class InteractButton : MonoBehaviour
{
    [Header("Activation")]
    [SerializeField] private ActivatorType acceptedActivators = ActivatorType.Either;
    [SerializeField] private string pressSfxId = "button_press";
    [SerializeField] private bool applyUpwardMomentum = true;
    [SerializeField] private float boostSpeed = 14f;
    [SerializeField] private float remoteBoostRadius = 1.25f;
    [SerializeField] private bool respawnOnGround = true;

    [Header("Visuals")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private Vector3 pressedOffset;

    [Header("Events")]
    public UnityEvent OnActivated;

    public bool IsActivated { get; private set; }
    public bool HasLatchedActivation { get; private set; }

    private Collider2D triggerCollider;
    private Sprite originalSprite;
    private Vector3 originalVisualLocalPosition;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();

        if (visual != null)
        {
            originalSprite = visual.sprite;
            originalVisualLocalPosition = visual.transform.localPosition;
        }
    }

    private void OnEnable()
    {
        GroundCheck.PlayerLandedOnGround += HandlePlayerLandedOnGround;
    }

    private void OnDisable()
    {
        GroundCheck.PlayerLandedOnGround -= HandlePlayerLandedOnGround;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsActivated) return;

        Activator activator = other.GetComponent<Activator>();
        if (activator == null) return;
        if (!Accepts(activator.type)) return;

        Activate(other, activator.type);
    }

    public void TryActivate(ActivatorType sourceType)
    {
        if (IsActivated || !Accepts(sourceType)) return;
        Activate(null, sourceType);
    }

    private void Activate(Collider2D sourceCollider, ActivatorType sourceType)
    {
        IsActivated = true;

        ApplyBoost(sourceCollider, sourceType);

        if (triggerCollider != null)
            triggerCollider.enabled = false;

        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(pressSfxId);
        else
            Debug.LogWarning("[InteractButton] AudioManager not found in scene.");

        animator?.SetTrigger("Pop");

        if (visual != null)
        {
            if (pressedSprite != null) visual.sprite = pressedSprite;
            visual.transform.localPosition += pressedOffset;
        }

        if (!HasLatchedActivation)
        {
            HasLatchedActivation = true;
            OnActivated.Invoke();
        }
    }

    private void ApplyBoost(Collider2D sourceCollider, ActivatorType sourceType)
    {
        if (!applyUpwardMomentum)
            return;

        PlayerLaunchReceiver launchReceiver = ResolveLaunchReceiver(sourceCollider, sourceType);
        if (launchReceiver != null)
            launchReceiver.LaunchUpward(boostSpeed);
    }

    private PlayerLaunchReceiver ResolveLaunchReceiver(Collider2D sourceCollider, ActivatorType sourceType)
    {
        if (sourceCollider != null)
        {
            PlayerLaunchReceiver directReceiver = sourceCollider.GetComponentInParent<PlayerLaunchReceiver>();
            if (directReceiver != null)
                return directReceiver;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return null;

        if (sourceType == ActivatorType.Star)
            return player.GetComponent<PlayerLaunchReceiver>();

        if (Vector2.Distance(transform.position, player.transform.position) > remoteBoostRadius)
            return null;

        return player.GetComponent<PlayerLaunchReceiver>();
    }

    private void HandlePlayerLandedOnGround()
    {
        if (!respawnOnGround || !IsActivated)
            return;

        IsActivated = false;

        if (triggerCollider != null)
            triggerCollider.enabled = true;

        if (visual != null)
        {
            visual.sprite = originalSprite;
            visual.transform.localPosition = originalVisualLocalPosition;
        }

        if (animator != null)
        {
            animator.ResetTrigger("Pop");
            animator.Play("Idle", 0, 0f);
            animator.Update(0f);
        }
    }

    private bool Accepts(ActivatorType incoming) => acceptedActivators switch
    {
        ActivatorType.Star   => incoming == ActivatorType.Star,
        ActivatorType.Player => incoming == ActivatorType.Player,
        _                    => true
    };
}
