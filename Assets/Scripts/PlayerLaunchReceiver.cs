using UnityEngine;
using Unity.VisualScripting;

[DefaultExecutionOrder(1000)]
public class PlayerLaunchReceiver : MonoBehaviour
{
    private Rigidbody2D rb;
    private GraphReference graphRef;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ScriptMachine scriptMachine = GetComponent<ScriptMachine>();
        if (scriptMachine != null)
            graphRef = GraphReference.New(scriptMachine, false);
    }

    public void LaunchUpward(float launchSpeed)
    {
        if (rb == null)
            return;

        Vector2 velocity = rb.velocity;
        velocity.y = launchSpeed;
        rb.velocity = velocity;

        if (graphRef == null)
            return;

        Variables.Graph(graphRef).Set("MovementMode", 1);
        Variables.Graph(graphRef).Set("VerticalVelocity", launchSpeed);
        Variables.Graph(graphRef).Set("JumpTimer", 0f);
    }
}
