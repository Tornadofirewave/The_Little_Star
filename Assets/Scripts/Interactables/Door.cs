using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Door : MonoBehaviour
{
    [Header("Gate")]
    [SerializeField] private List<InteractButton> requiredButtons = new();

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 1.2f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Audio")]
    [SerializeField] private string openSfxId = "door_open";

    [Header("Options")]
    [SerializeField] private bool disableColliderWhenOpen = true;

    private bool _opened;
    private Tilemap _tilemap;

    private void Start()
    {
        foreach (var btn in requiredButtons)
            btn.OnActivated.AddListener(OnButtonActivated);

        _tilemap = GetComponentInChildren<Tilemap>();
    }

    private void OnDestroy()
    {
        foreach (var btn in requiredButtons)
            if (btn != null) btn.OnActivated.RemoveListener(OnButtonActivated);
    }

    private void OnButtonActivated()
    {
        if (_opened) return;
        if (requiredButtons.All(b => b.HasLatchedActivation))
            StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        _opened = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(openSfxId);

        if (disableColliderWhenOpen)
        {
            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }

        float elapsed = 0f;
        Color color = _tilemap.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = 1f - fadeCurve.Evaluate(Mathf.Clamp01(elapsed / fadeDuration));
            _tilemap.color = color;
            yield return null;
        }

        color.a = 0f;
        _tilemap.color = color;
    }
}
