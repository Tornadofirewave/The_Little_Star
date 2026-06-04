using System;
using UnityEngine;
using Unity.VisualScripting;

public class GroundCheck : MonoBehaviour
{
    public static event Action PlayerLandedOnGround;

    [SerializeField] private float checkDistance = 0.05f;
    [SerializeField] private LayerMask groundLayer;

    private GraphReference graphRef;
    private Collider2D col;
    private readonly RaycastHit2D[] castHits = new RaycastHit2D[8];

    private void Start()
    {
        graphRef = GraphReference.New(GetComponent<ScriptMachine>(), false);
        col = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        int mode = Variables.Graph(graphRef).Get<int>("MovementMode");
        int mask = groundLayer.value == 0 ? ~0 : (int)groundLayer;
        Vector2 castSize = new Vector2(col.bounds.size.x * 0.9f, col.bounds.size.y);
        RaycastHit2D hit = BoxCastIgnoringTriggers(Vector2.down, castSize, mask);
        bool grounded = hit && hit.normal.y > 0.5f;

        RaycastHit2D ceilingHit = BoxCastIgnoringTriggers(Vector2.up, castSize, mask);
        bool hittingCeiling = ceilingHit && ceilingHit.normal.y < -0.5f && !IsOneWayPlatform(ceilingHit.collider);

        if (grounded && mode == 3)
        {
            PlayerLandedOnGround?.Invoke();
            Variables.Graph(graphRef).Set("MovementMode", 0);
            Variables.Graph(graphRef).Set("VerticalVelocity", 0f);
        }
        else if (!grounded && mode == 0)
        {
            Variables.Graph(graphRef).Set("MovementMode", 3);
        }

        if (hittingCeiling && (mode == 1 || mode == 2))
        {
            Variables.Graph(graphRef).Set("MovementMode", 3);
            Variables.Graph(graphRef).Set("VerticalVelocity", 0f);
        }
    }

    private RaycastHit2D BoxCastIgnoringTriggers(Vector2 direction, Vector2 castSize, int mask)
    {
        int hitCount = Physics2D.BoxCastNonAlloc(col.bounds.center, castSize, 0f, direction, castHits, checkDistance, mask);

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hitCollider = castHits[i].collider;
            if (hitCollider != null && !hitCollider.isTrigger)
                return castHits[i];
        }

        return default;
    }

    private static bool IsOneWayPlatform(Collider2D target)
    {
        if (target == null)
            return false;

        return target.GetComponent<PlatformEffector2D>() != null ||
               target.GetComponent<OneWayPlatform>() != null;
    }
}
