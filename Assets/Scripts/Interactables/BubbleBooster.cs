using UnityEngine;

public class BubbleBooster : MonoBehaviour
{
    public static event System.Action OnFirstActivated;
    private static bool _firstFired;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetStaticState() => _firstFired = false;

    [Header("Activation")]
    [SerializeField] private ActivatorType acceptedActivators = ActivatorType.Either;
    [SerializeField] private string popSfxId = "button_press";
    [SerializeField] private bool singleUse = true;

    [Header("Boost")]
    [SerializeField] private float boostSpeed = 16f;
    [SerializeField] private float remoteBoostRadius = 1.25f;

    [Header("Visuals")]
    [SerializeField] private Animator animator;

    public bool IsActivated { get; private set; }

    private Collider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
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
        if (IsActivated && singleUse) return;

        Activator activator = other.GetComponent<Activator>();
        if (activator == null) return;
        if (!Accepts(activator.type)) return;

        Activate(other);
    }

    public void TryActivate(ActivatorType sourceType)
    {
        if (IsActivated && singleUse) return;
        if (!Accepts(sourceType)) return;

        Activate(null, sourceType);
    }

    private void Activate(Collider2D sourceCollider)
    {
        Activate(sourceCollider, sourceCollider != null ? sourceCollider.GetComponent<Activator>()?.type ?? ActivatorType.Either : ActivatorType.Either);
    }

    private void Activate(Collider2D sourceCollider, ActivatorType sourceType)
    {
        if (!_firstFired)
        {
            _firstFired = true;
            OnFirstActivated?.Invoke();
        }

        PlayerLaunchReceiver launchReceiver = ResolveLaunchReceiver(sourceCollider, sourceType);
        if (launchReceiver != null)
            launchReceiver.LaunchUpward(boostSpeed);

        if (singleUse)
        {
            IsActivated = true;
            if (triggerCollider != null)
                triggerCollider.enabled = false;
        }

        AudioManager.Instance?.Play(popSfxId);
        animator?.SetTrigger("Pop");
    }

    private void HandlePlayerLandedOnGround()
    {
        Respawn();
    }

    private void Respawn()
    {
        IsActivated = false;

        if (triggerCollider != null)
            triggerCollider.enabled = true;

        if (animator == null)
            return;

        animator.ResetTrigger("Pop");
        animator.Play("Idle", 0, 0f);
        animator.Update(0f);
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

    private bool Accepts(ActivatorType incoming) => acceptedActivators switch
    {
        ActivatorType.Star => incoming == ActivatorType.Star,
        ActivatorType.Player => incoming == ActivatorType.Player,
        _ => true
    };
}
