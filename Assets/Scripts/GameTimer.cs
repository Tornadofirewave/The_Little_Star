using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private float _elapsed;
    private bool _running;

    private void OnEnable()  => BubbleBooster.OnFirstActivated += StartTimer;
    private void OnDisable() => BubbleBooster.OnFirstActivated -= StartTimer;

    private void StartTimer() => _running = true;

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
