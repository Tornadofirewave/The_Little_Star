using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class OneWayPlatform : MonoBehaviour
{
    [SerializeField] private float surfaceArc = 180f;
    [SerializeField] private float sideArc = 0f;
    [SerializeField] private bool useOneWayGrouping = true;

    private void Awake()
    {
        EnsureEffectorExists();
        ConfigureEffector();
    }

    private void Reset()
    {
        EnsureEffectorExists();
        ConfigureEffector();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
            ConfigureEffector();
    }
#endif

    private void EnsureEffectorExists()
    {
        if (GetComponent<PlatformEffector2D>() == null)
            gameObject.AddComponent<PlatformEffector2D>();
    }

    private void ConfigureEffector()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
            return;

        PlatformEffector2D effector = GetComponent<PlatformEffector2D>();
        if (effector == null)
            return;

        boxCollider.usedByEffector = true;

        effector.useOneWay = true;
        effector.useOneWayGrouping = useOneWayGrouping;
        effector.surfaceArc = surfaceArc;
        effector.sideArc = sideArc;
        effector.rotationalOffset = 0f;
    }
}
