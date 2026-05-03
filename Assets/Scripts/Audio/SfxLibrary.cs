using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SfxLibrary", menuName = "Audio/Sfx Library")]
public class SfxLibrary : ScriptableObject
{
    [Serializable]
    public class SfxEntry
    {
        public string id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        public Vector2 pitchRange = Vector2.one;
    }

    [SerializeField] private List<SfxEntry> entries = new();

    private Dictionary<string, SfxEntry> _lookup;

    private void OnEnable() => BuildLookup();

    public bool TryGet(string id, out SfxEntry entry)
    {
        if (_lookup == null) BuildLookup();
        return _lookup.TryGetValue(id, out entry);
    }

    private void BuildLookup()
    {
        _lookup = new Dictionary<string, SfxEntry>(entries.Count);
        foreach (var e in entries)
        {
            if (!string.IsNullOrEmpty(e.id))
                _lookup[e.id] = e;
        }
    }
}
