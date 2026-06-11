using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioClip bgmClip;
    [SerializeField] [Range(0f, 1f)] private float volume = 0.5f;

    private AudioSource _source;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _source = gameObject.AddComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.loop = true;
        _source.volume = volume;
    }

    private void Start()
    {
        if (bgmClip != null)
            Play(bgmClip);
    }

    public void Play(AudioClip clip)
    {
        _source.clip = clip;
        _source.Play();
    }

    public void Stop() => _source.Stop();

    public void SetVolume(float v) => _source.volume = Mathf.Clamp01(v);
}
