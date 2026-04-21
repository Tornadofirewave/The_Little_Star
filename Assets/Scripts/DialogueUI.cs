using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    public void Show()  => dialoguePanel.SetActive(true);
    public void Hide()  { dialoguePanel.SetActive(false); dialogueText.text = ""; }
    public void SetText(string text)  => dialogueText.text = text;
    public void AppendChar(char c)    => dialogueText.text += c;
}
