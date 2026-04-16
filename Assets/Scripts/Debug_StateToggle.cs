using UnityEngine;
using Unity.VisualScripting;

// TEMPORARY — delete after Phase 1 verification
public class Debug_StateToggle : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private bool _inDialogue;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _inDialogue = !_inDialogue;
            string evt = _inDialogue ? "StartDialogue" : "EndDialogue";
            CustomEvent.Trigger(player, evt);
            Debug.Log($"Fired {evt}");
        }
    }
}
