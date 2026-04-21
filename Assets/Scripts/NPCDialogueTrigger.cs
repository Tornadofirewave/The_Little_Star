using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Content")]
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private string repeatLine = "...";

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.UpArrow;

    private bool _playerInRange;
    private bool _hasSpokenBefore;
    private bool _isDialogueActive;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) _playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            _isDialogueActive = false;
        }
    }

    private void Update()
    {
        if (!_playerInRange || _isDialogueActive) return;
        if (!Input.GetKeyDown(interactKey)) return;

        string[] linesToShow = _hasSpokenBefore
            ? new string[] { repeatLine }
            : dialogueLines;

        _isDialogueActive = true;
        _hasSpokenBefore = true;
        DialogueManager.Instance.StartDialogue(linesToShow, OnDialogueEnded);
    }

    private void OnDialogueEnded() => _isDialogueActive = false;
}
