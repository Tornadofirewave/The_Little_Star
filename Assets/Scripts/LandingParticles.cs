using UnityEngine;
using Unity.VisualScripting;

public class LandingParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem landingEffect;
    [SerializeField] private float fallThreshold = 2.5f;

    private GraphReference graphRef;
    private int prevMode;
    private float peakY;
    private bool trackingFall;

    private void Start()
    {
        graphRef = GraphReference.New(GetComponent<ScriptMachine>(), false);
        prevMode = Variables.Graph(graphRef).Get<int>("MovementMode");
    }

    private void Update()
    {
        int mode = Variables.Graph(graphRef).Get<int>("MovementMode");

        // Left the ground — begin tracking peak height
        if (prevMode == 0 && mode != 0)
        {
            peakY = transform.position.y;
            trackingFall = true;
        }

        // While airborne, keep the highest point reached
        if (trackingFall && mode != 0)
        {
            if (transform.position.y > peakY)
                peakY = transform.position.y;
        }

        // Landed — check how far we fell from the peak
        if (prevMode != 0 && mode == 0)
        {
            if (trackingFall)
            {
                float fallDistance = peakY - transform.position.y;
                if (fallDistance >= fallThreshold)
                {
                    landingEffect.gameObject.SetActive(true);
                    landingEffect.Play();
                }
            }
            trackingFall = false;
        }

        prevMode = mode;
    }
}
