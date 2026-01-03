using UnityEngine;

public class NoteScroller : MonoBehaviour
{
    // Set speed to scroll notes
    [SerializeField] private float _scrollSpeed;

    // Reference music player to sync scrolling with music
    [SerializeField] private MusicPlayer _musicPlayer;

    // Reference object to move / scroll
    [SerializeField] private Transform _notesGroup;
    
    private float _initialPosition;

    private void Awake()
    {
        _initialPosition = _notesGroup.localPosition.x;
    }

    void Update()
    {
        // Calculate position offset based on song time & scroll speed
        float _songTime = _musicPlayer.GetTimeInSeconds();
        float _newPositionOffset = _scrollSpeed * _songTime;

        // Calculate position based on inital position & position offset
        float _newPosition = _initialPosition - _newPositionOffset;

        // Set new position
        _notesGroup.localPosition = new Vector3(_newPosition,
            _notesGroup.localPosition.y, _notesGroup.localPosition.z);
    }

    public float GetScrollSpeed()
    {
        return _scrollSpeed;
    }
}
