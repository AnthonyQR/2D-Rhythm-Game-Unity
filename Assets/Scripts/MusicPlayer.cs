using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // Audio source & song references
    [SerializeField] private AudioSource _audioPlayer;
    [SerializeField] private AudioClip _songToPlay;
    [SerializeField] private float _startTime;
    [SerializeField] private float _initialDelay;
    [SerializeField] private float _songBPM;

    // Tracked in different formats to be referenced by other components
    private float _timeInSongInSeconds;
    private float _timeInSongInSecondsWithoutDelay;
    private float _timeInBeats;

    void Start ()
    {
        // Set audio clip & play song
        _audioPlayer.clip = _songToPlay;
        _audioPlayer.time = _startTime;
        _audioPlayer.Play();
        
    }

    void Update()
    {
        // Constantly update the time in the song
        _timeInSongInSeconds = ((float)_audioPlayer.timeSamples / _songToPlay.frequency);
        _timeInSongInSecondsWithoutDelay = _timeInSongInSeconds - _initialDelay;
        _timeInBeats = _timeInSongInSecondsWithoutDelay * _songBPM / 60;
    }

    public float GetTimeInSeconds()
    {
        return _timeInSongInSeconds;
    }

    public float GetTimeInSecondsWithoutDelay()
    {
        return _timeInSongInSecondsWithoutDelay;
    }
    public float GetTimeInBeats()
    {
        return _timeInBeats;
    }

    public float GetInitialDelayInBeats()
    {
        return _initialDelay * (_songBPM / 60);
    }

    public float GetBPM()
    {
        return _songBPM;
    }
}
