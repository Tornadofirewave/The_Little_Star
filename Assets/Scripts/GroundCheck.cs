using UnityEngine;
using Unity.VisualScripting;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private float checkDistance = 0.05f;
    [SerializeField] private LayerMask groundLayer;

    private GraphReference graphRef;
    private Collider2D col;

    private void Start()
    {
        graphRef = GraphReference.New(GetComponent<ScriptMachine>(), false);
        col = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        int mode = Variables.Graph(graphRef).Get<int>("MovementMode");
        int mask = groundLayer.value == 0 ? ~0 : (int)groundLayer;
        RaycastHit2D hit = Physics2D.BoxCast(col.bounds.center, new Vector2(col.bounds.size.x * 0.9f, col.bounds.size.y), 0f, Vector2.down, checkDistance, mask);
        bool grounded = hit && hit.normal.y > 0.5f;

        RaycastHit2D ceilingHit = Physics2D.BoxCast(col.bounds.center, new Vector2(col.bounds.size.x * 0.9f, col.bounds.size.y), 0f, Vector2.up, checkDistance, mask);
        bool hittingCeiling = ceilingHit && ceilingHit.normal.y < -0.5f;

        if (grounded && mode == 3)
        {
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
}
