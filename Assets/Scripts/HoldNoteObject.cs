using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class HoldNoteObject : MonoBehaviour
{
    [SerializeField] private RectTransform _barObject;
    [SerializeField] private Slider _barFill;
    [SerializeField] private float _destroyTimer;
    [SerializeField] private RectTransform _imageObject;
    private NoteScroller _noteScroller;
    private MusicPlayer _musicPlayer;
    private Note _noteInfo;

    public Note SetupHoldNote(Note note, NoteScroller _newNoteScroller, MusicPlayer _newMusicPlayer)
    {
        _noteScroller = _newNoteScroller;
        float scrollSpeed = _noteScroller.GetScrollSpeed();

        _musicPlayer = _newMusicPlayer;
        float beatsPerSecond = _musicPlayer.GetBPM() / 60;

        float _barWidth = (note.holdDuration * scrollSpeed) / beatsPerSecond;

        _barObject.sizeDelta = new Vector2(_barWidth, _barObject.sizeDelta.y);
        _barFill.maxValue = _barWidth;
        _barFill.value = _barWidth;

        
        _noteInfo = note;
        _noteInfo.holdNoteScript = this;
        return _noteInfo;
    }

    // Move note image & bar fill
    public void UpdateHoldNoteVisuals()
    {
        // Calculate beat to move the image to
        float noteBeatOffset = _musicPlayer.GetInitialDelayInBeats();
        float currentBeat = _musicPlayer.GetTimeInBeats();
        float beatToMoveTo = currentBeat - _noteInfo.beat;

        // Calculate position based on scroll speed
        float scrollSpeedPosition = _noteScroller.GetScrollSpeed() * beatToMoveTo;

        // Get bps for position calculations
        float beatsPerSecond = _musicPlayer.GetBPM() / 60;

        // Calculate position based on scroll speed AND song bps
        float newNotePosition = scrollSpeedPosition / beatsPerSecond;

        // Move note image to new position
        _imageObject.localPosition = new Vector3(newNotePosition, 0f, 0f);

        float holdProgress = beatToMoveTo / _noteInfo.holdDuration;
        _barFill.value = _barFill.maxValue - (_barFill.maxValue * holdProgress) ;
    }

    public void UpdatePointsScored(int pointsScored)
    {
        _noteInfo.pointsScored = pointsScored;
    }

    public int GetPointsScored()
    {
        return _noteInfo.pointsScored;
    }

    // Destroy self after a timer
    public void StartDestroy()
    {
        StartCoroutine(DestroyTimer());
    }

    IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(_destroyTimer);
        Destroy(this.gameObject);
    }
}
