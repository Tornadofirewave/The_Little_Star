using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

[DefaultExecutionOrder(-1000)]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDropThroughPlatform : MonoBehaviour
{
    [SerializeField] private float verticalInputThreshold = -0.5f;
    [SerializeField] private float castDistance = 0.12f;
    [SerializeField] private float dropVelocity = 2f;
    [SerializeField] private float ignoreDuration = 0.2f;

    private readonly RaycastHit2D[] castHits = new RaycastHit2D[8];
    private readonly HashSet<Collider2D> ignoredColliders = new HashSet<Collider2D>();

    private Collider2D playerCollider;
    private Rigidbody2D rb;
    private GraphReference graphRef;

    private void Start()
    {
        playerCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        ScriptMachine scriptMachine = GetComponent<ScriptMachine>();
        if (scriptMachine != null)
            graphRef = GraphReference.New(scriptMachine, false);
    }

    private void Update()
    {
        var objVars = Variables.Object(gameObject);
        if (objVars.IsDefined("InDialogue") && objVars.Get<bool>("InDialogue")) return;
        if (!Input.GetButtonDown("Jump")) return;
        if (Input.GetAxisRaw("Vertical") > verticalInputThreshold) return;
        if (graphRef == null) return;
        if (Variables.Graph(graphRef).Get<int>("MovementMode") != 0) return;

        if (!TryDropThroughPlatform())
            return;

        Variables.Graph(graphRef).Set("MovementMode", 3);
        Variables.Graph(graphRef).Set("VerticalVelocity", -dropVelocity);
        rb.velocity = new Vector2(rb.velocity.x, -dropVelocity);
    }

    private void OnDisable()
    {
        foreach (Collider2D collider in ignoredColliders)
        {
            if (collider != null && playerCollider != null)
                Physics2D.IgnoreCollision(playerCollider, collider, false);
        }

        ignoredColliders.Clear();
    }

    private bool TryDropThroughPlatform()
    {
        if (playerCollider == null)
            return false;

        ContactFilter2D filter = new ContactFilter2D
        {
            useTriggers = false,
            useLayerMask = false
        };

        int hitCount = playerCollider.Cast(Vector2.down, filter, castHits, castDistance);
        bool ignoredAny = false;

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D targetCollider = castHits[i].collider;
            if (targetCollider == null)
                continue;

            if (targetCollider.GetComponent<PlatformEffector2D>() == null &&
                targetCollider.GetComponent<OneWayPlatform>() == null)
            {
                continue;
            }

            if (ignoredColliders.Add(targetCollider))
            {
                Physics2D.IgnoreCollision(playerCollider, targetCollider, true);
                StartCoroutine(RestoreCollisionAfterDelay(targetCollider));
            }

            ignoredAny = true;
        }

        return ignoredAny;
    }

    private IEnumerator RestoreCollisionAfterDelay(Collider2D targetCollider)
    {
        yield return new WaitForSeconds(ignoreDuration);

        if (targetCollider != null && playerCollider != null)
            Physics2D.IgnoreCollision(playerCollider, targetCollider, false);

        ignoredColliders.Remove(targetCollider);
    }
}
