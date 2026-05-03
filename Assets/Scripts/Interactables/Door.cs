using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Gate")]
    [SerializeField] private List<InteractButton> requiredButtons = new();

    [Header("Motion")]
    [SerializeField] private float raiseDistance = 4f;
    [SerializeField] private float raiseDuration = 1.2f;
    [SerializeField] private AnimationCurve raiseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Audio")]
    [SerializeField] private string raiseSfxId = "door_raise";

    [Header("Options")]
    [SerializeField] private bool disableColliderWhenOpen = true;

    private bool _raised;

    private void Start()
    {
        foreach (var btn in requiredButtons)
            btn.OnActivated.AddListener(OnButtonActivated);
    }

    private void OnDestroy()
    {
        foreach (var btn in requiredButtons)
            if (btn != null) btn.OnActivated.RemoveListener(OnButtonActivated);
    }

    private void OnButtonActivated()
    {
        if (_raised) return;
        if (requiredButtons.All(b => b.IsActivated))
            StartCoroutine(RaiseRoutine());
    }

    private IEnumerator RaiseRoutine()
    {
        _raised = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.Play(raiseSfxId);

        Vector3 start = transform.position;
        Vector3 end = start + Vector3.up * raiseDistance;
        float elapsed = 0f;

        while (elapsed < raiseDuration)
        {
            elapsed += Time.deltaTime;
            float t = raiseCurve.Evaluate(Mathf.Clamp01(elapsed / raiseDuration));
            transform.position = Vector3.LerpUnclamped(start, end, t);
            yield return null;
        }

        transform.position = end;

        if (disableColliderWhenOpen)
        {
            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }
    }
}
