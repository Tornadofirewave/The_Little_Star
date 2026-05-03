using UnityEngine;
using UnityEngine.Events;

public class InteractButton : MonoBehaviour
{
    [Header("Activation")]
    [SerializeField] private ActivatorType acceptedActivators = ActivatorType.Either;
    [SerializeField] private string pressSfxId = "button_press";

    [Header("Visuals")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer visual;
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private Vector3 pressedOffset;

    [Header("Events")]
    public UnityEvent OnActivated;

    public bool IsActivated { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsActivated) return;

        Activator activator = other.GetComponent<Activator>();
        if (activator == null) return;
        if (!Accepts(activator.type)) return;

        Activate();
    }

    public void TryActivate(ActivatorType sourceType)
    {
        if (IsActivated || !Accepts(sourceType)) return;
        Activate();
    }

    private void Activate()
    {
        IsActivated = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

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

        OnActivated.Invoke();
    }

    private bool Accepts(ActivatorType incoming) => acceptedActivators switch
    {
        ActivatorType.Star   => incoming == ActivatorType.Star,
        ActivatorType.Player => incoming == ActivatorType.Player,
        _                    => true
    };
}
