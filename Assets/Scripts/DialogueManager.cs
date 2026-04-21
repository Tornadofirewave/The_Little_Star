using System.Collections;
using UnityEngine;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private GameObject playerObject;

    [Header("Typewriter")]
    [SerializeField] private float typewriterSpeed = 0.04f;

    private string[] _currentLines;
    private int _currentLineIndex;
    private bool _isTyping;
    private bool _dialogueActive;
    private Coroutine _typewriterCoroutine;
    private System.Action _onCompleteCallback;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartDialogue(string[] lines, System.Action onComplete = null)
    {
        if (_dialogueActive) return;
        _currentLines = lines;
        _currentLineIndex = 0;
        _onCompleteCallback = onComplete;
        _dialogueActive = true;
        dialogueUI.Show();
        CustomEvent.Trigger(playerObject, "StartDialogue");
        OnDialogueTriggeredEvent.Trigger(gameObject);
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (_typewriterCoroutine != null) StopCoroutine(_typewriterCoroutine);
        _typewriterCoroutine = StartCoroutine(TypewriterRoutine(_currentLines[_currentLineIndex]));
    }

    private IEnumerator TypewriterRoutine(string line)
    {
        _isTyping = true;
        dialogueUI.SetText("");
        foreach (char c in line)
        {
            dialogueUI.AppendChar(c);
            yield return new WaitForSeconds(typewriterSpeed);
        }
        _isTyping = false;
    }

    private void Update()
    {
        if (!_dialogueActive) return;
        if (!Input.GetKeyDown(KeyCode.Z)) return;

        if (_isTyping)
            SkipTypewriter();
        else if (_currentLineIndex < _currentLines.Length - 1)
            AdvanceLine();
        else
            EndDialogue();
    }

    private void SkipTypewriter()
    {
        StopCoroutine(_typewriterCoroutine);
        dialogueUI.SetText(_currentLines[_currentLineIndex]);
        _isTyping = false;
    }

    private void AdvanceLine()
    {
        _currentLineIndex++;
        ShowCurrentLine();
    }

    private void EndDialogue()
    {
        _dialogueActive = false;
        dialogueUI.Hide();
        CustomEvent.Trigger(playerObject, "EndDialogue");
        _onCompleteCallback?.Invoke();
        _onCompleteCallback = null;
    }
}
