using UnityEngine;
using Unity.VisualScripting;

public class JumpBuffer : MonoBehaviour
{
    [SerializeField] private float bufferWindow = 0.12f;
    [SerializeField] private float shortHopMultiplier = 0.5f;
    [SerializeField] private float coyoteWindow = 0.1f;

    private float bufferTimer;
    private float coyoteTimer;
    private int prevMode;
    private GraphReference graphRef;

    private void Start()
    {
        graphRef = GraphReference.New(GetComponent<ScriptMachine>(), false);
    }

    private void Update()
    {
        var objVars = Variables.Object(gameObject);
        if (objVars.IsDefined("InDialogue") && objVars.Get<bool>("InDialogue")) return;

        int mode = Variables.Graph(graphRef).Get<int>("MovementMode");

        // Start coyote window when walking off an edge (grounded -> falling)
        if (prevMode == 0 && mode == 3)
            coyoteTimer = coyoteWindow;

        if (coyoteTimer > 0f)
            coyoteTimer -= Time.deltaTime;

        // Buffer jump pressed while airborne
        if (Input.GetButtonDown("Jump") && mode != 0)
        {
            if (coyoteTimer > 0f)
            {
                coyoteTimer = 0f;
                ExecuteJump();
            }
            else
            {
                bufferTimer = bufferWindow;
            }
        }

        if (bufferTimer > 0f)
            bufferTimer -= Time.deltaTime;

        if (prevMode != 0 && mode == 0 && bufferTimer > 0f)
            ExecuteJump();

        prevMode = mode;
    }

    private void ExecuteJump()
    {
        bufferTimer = 0f;
        float jumpMaxSpeed = Variables.Graph(graphRef).Get<float>("JumpMaxSpeed");

        if (Input.GetButton("Jump"))
        {
            Variables.Graph(graphRef).Set("MovementMode", 1);
            Variables.Graph(graphRef).Set("VerticalVelocity", jumpMaxSpeed);
            Variables.Graph(graphRef).Set("JumpTimer", 0f);
        }
        else
        {
            Variables.Graph(graphRef).Set("MovementMode", 3);
            Variables.Graph(graphRef).Set("VerticalVelocity", jumpMaxSpeed * shortHopMultiplier);
            Variables.Graph(graphRef).Set("JumpTimer", 0f);
        }
    }
}
