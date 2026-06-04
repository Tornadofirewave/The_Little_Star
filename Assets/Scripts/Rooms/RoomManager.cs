using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }


    [SerializeField] private List<Room> rooms = new();

    private int _currentIndex = 0;
    private readonly HashSet<Room> _faded = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Called by RoomTransitionTrigger when the player passes upward into a room.
    public void AdvanceFrom(Room enteredRoom)
    {
        int enteredIndex = rooms.IndexOf(enteredRoom);
        if (enteredIndex <= _currentIndex || enteredIndex < 0)
            return;

        _currentIndex = enteredIndex;

        // Fade out all rooms before the current one that haven't been faded yet.
        for (int i = 0; i < _currentIndex; i++)
        {
            Room room = rooms[i];
            if (room == null || _faded.Contains(room)) continue;
            _faded.Add(room);
            StartCoroutine(room.FadeOut());
        }
    }
}
