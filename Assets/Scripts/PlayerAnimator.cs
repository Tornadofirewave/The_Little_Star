using UnityEngine;
using Unity.VisualScripting;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        var objVars = Variables.Object(gameObject);
        if (objVars.IsDefined("InDialogue") && objVars.Get<bool>("InDialogue")) return;

        float h = Input.GetAxisRaw("Horizontal");

        animator.SetFloat(SpeedHash, Mathf.Abs(h));

        if (h != 0f)
        {
            Vector3 scale = transform.localScale;
            scale.x = h > 0f ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
}
