using UnityEngine;
using Unity.VisualScripting;

public class BackgroundFollower : MonoBehaviour
{
    [Header("Destroy Threshold")]
    [Tooltip("X world position at which this object destroys itself (fallback to Timeline signal).")]
    [SerializeField] private float destroyAtX = -50f;

    private Rigidbody2D _playerRb;
    private GameObject _playerObj;

    public void Initialize(Rigidbody2D playerRb)
    {
        _playerRb = playerRb;
        if (playerRb != null)
            _playerObj = playerRb.gameObject;
    }

    private void Update()
    {
        if (_playerRb == null) return;

        bool inDialogue = false;
        if (_playerObj != null)
        {
            var objVars = Variables.Object(_playerObj);
            if (objVars.IsDefined("InDialogue"))
                inDialogue = objVars.Get<bool>("InDialogue");
        }

        if (!inDialogue)
        {
            transform.position += new Vector3(_playerRb.velocity.x * Time.deltaTime, 0f, 0f);
        }

        if (transform.position.x < destroyAtX)
            Destroy(gameObject);
    }

    public void DestroySelf() => Destroy(gameObject);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(destroyAtX, -100f, 0f), new Vector3(destroyAtX, 100f, 0f));
    }
}
