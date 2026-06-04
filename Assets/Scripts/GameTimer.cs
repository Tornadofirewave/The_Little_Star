using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private float _elapsed;
    private bool _running;

    private void OnEnable()
    {
        BubbleBooster.OnActivated              += HandleBubble;
        RoomTransitionTrigger.OnPlayerCrossed  += HandleTransition;
    }

    private void OnDisable()
    {
        BubbleBooster.OnActivated              -= HandleBubble;
        RoomTransitionTrigger.OnPlayerCrossed  -= HandleTransition;
    }

    // First bubble of each level: reset and start.
    private void HandleBubble()
    {
        if (_running) return;
        _elapsed = 0f;
        _running = true;
    }

    // Crossing a transition: pause.
    private void HandleTransition()
    {
        _running = false;
    }

    private void Update()
    {
        if (!_running) return;
        _elapsed += Time.deltaTime;
        timerText.text = FormatTime(_elapsed);
    }

    private static string FormatTime(float t)
    {
        int minutes = (int)(t / 60);
        int seconds = (int)(t % 60);
        int centis  = (int)((t * 100f) % 100);
        return $"{minutes}:{seconds:00}.{centis:00}";
    }
}
