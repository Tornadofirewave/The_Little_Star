using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private SpriteRenderer[] _spriteRenderers;
    private Tilemap[] _tilemaps;
    private MaterialPropertyBlock _mpb;

    private void Awake()
    {
        Refresh();
        _mpb = new MaterialPropertyBlock();
    }

    // Call if renderers are added at runtime.
    public void Refresh()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        _tilemaps = GetComponentsInChildren<Tilemap>(true);
    }

    public IEnumerator FadeOut() => Fade(0f);
    public IEnumerator FadeIn() => Fade(1f);

    private IEnumerator Fade(float targetAlpha)
    {
        float elapsed = 0f;
        float[] startAlphas = CaptureAlphas();

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = fadeCurve.Evaluate(Mathf.Clamp01(elapsed / fadeDuration));

            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                if (_spriteRenderers[i] == null) continue;
                _spriteRenderers[i].GetPropertyBlock(_mpb);
                Color c = _spriteRenderers[i].color;
                c.a = Mathf.Lerp(startAlphas[i], targetAlpha, t);
                _mpb.SetColor("_Color", c);
                _spriteRenderers[i].SetPropertyBlock(_mpb);
            }

            int offset = _spriteRenderers.Length;
            for (int i = 0; i < _tilemaps.Length; i++)
            {
                if (_tilemaps[i] == null) continue;
                Color c = _tilemaps[i].color;
                c.a = Mathf.Lerp(startAlphas[offset + i], targetAlpha, t);
                _tilemaps[i].color = c;
            }

            yield return null;
        }

        // Snap to final value.
        SetAllAlpha(targetAlpha);
    }

    private float[] CaptureAlphas()
    {
        var alphas = new float[_spriteRenderers.Length + _tilemaps.Length];
        for (int i = 0; i < _spriteRenderers.Length; i++)
            alphas[i] = _spriteRenderers[i] != null ? _spriteRenderers[i].color.a : 1f;
        for (int i = 0; i < _tilemaps.Length; i++)
            alphas[_spriteRenderers.Length + i] = _tilemaps[i] != null ? _tilemaps[i].color.a : 1f;
        return alphas;
    }

    private void SetAllAlpha(float alpha)
    {
        foreach (var sr in _spriteRenderers)
        {
            if (sr == null) continue;
            sr.GetPropertyBlock(_mpb);
            Color c = sr.color;
            c.a = alpha;
            _mpb.SetColor("_Color", c);
            sr.SetPropertyBlock(_mpb);
        }
        foreach (var tm in _tilemaps)
        {
            if (tm == null) continue;
            Color c = tm.color;
            c.a = alpha;
            tm.color = c;
        }
    }
}
