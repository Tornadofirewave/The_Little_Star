using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private SfxLibrary library;
    [SerializeField] private int poolSize = 8;

    private readonly List<AudioSource> _pool = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildPool();
    }

    public void Play(string id)
    {
        if (!Resolve(id, out var entry, out var src)) return;
        Configure(src, entry);
        src.Play();
    }

    public void PlayAt(string id, Vector3 worldPos)
    {
        if (!Resolve(id, out var entry, out var src)) return;
        Configure(src, entry);
        src.transform.position = worldPos;
        src.Play();
    }

    public void Stop(string id)
    {
        foreach (var src in _pool)
        {
            if (src.isPlaying && src.clip != null && src.clip.name == id)
                src.Stop();
        }
    }

    private bool Resolve(string id, out SfxLibrary.SfxEntry entry, out AudioSource src)
    {
        src = null;
        if (!library.TryGet(id, out entry))
        {
            Debug.LogWarning($"[AudioManager] Unknown SFX id: '{id}'");
            return false;
        }
        src = GetFreeSource();
        return true;
    }

    private void Configure(AudioSource src, SfxLibrary.SfxEntry entry)
    {
        if (entry.clip == null)
        {
            Debug.LogWarning($"[AudioManager] Clip is null for id: '{entry.id}'");
            return;
        }
        src.clip = entry.clip;
        src.volume = entry.volume;
        src.pitch = Random.Range(entry.pitchRange.x, entry.pitchRange.y);
        Debug.Log($"[AudioManager] Playing '{entry.id}' on {src.name}, clip={entry.clip.name}, vol={src.volume}");
    }

    private AudioSource GetFreeSource()
    {
        foreach (var s in _pool)
            if (!s.isPlaying) return s;
        // pool exhausted — reuse oldest
        return _pool[0];
    }

    private void BuildPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject($"SfxSource_{i}");
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            _pool.Add(src);
        }
    }
}
