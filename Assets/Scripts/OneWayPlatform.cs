using UnityEngine;
using UnityEngine.Tilemaps;

// Works with BoxCollider2D (standard platform) or TilemapCollider2D (tilemap platform).
// Do NOT add CompositeCollider2D to a one-way platform — it is incompatible with PlatformEffector2D.
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
        PlatformEffector2D effector = GetComponent<PlatformEffector2D>();
        if (effector == null)
            return;

        // Apply usedByEffector to whichever collider type is present.
        var box = GetComponent<BoxCollider2D>();
        if (box != null) box.usedByEffector = true;

        var tilemap = GetComponent<TilemapCollider2D>();
        if (tilemap != null) tilemap.usedByEffector = true;

        effector.useOneWay = true;
        effector.useOneWayGrouping = useOneWayGrouping;
        effector.surfaceArc = surfaceArc;
        effector.sideArc = sideArc;
        effector.rotationalOffset = 0f;
    }
}
