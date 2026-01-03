using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoteJudger : MonoBehaviour
{
    // Float to determine the timing window for note to be valid
    [SerializeField] private float _noteTimingWindowInSeconds;

    // Referece components for calculation judgement area
    [SerializeField] private MusicPlayer _musicPlayer;

    [SerializeField] private UIManager _uiManager;

    // List of notes for each row to check individually
    private List<Note> _firstRowNotes;
    private List<Note> _secondRowNotes;
    private List<Note> _thirdRowNotes;
    private List<Note> _fourthRowNotes;

    // Input actions for each row
    private PlayerInput _playerInput;
    private InputAction _firstRowInput;
    private InputAction _secondRowInput;
    private InputAction _thirdRowInput;
    private InputAction _fourthRowInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();

        _firstRowInput = _playerInput.Player.FirstRow;
        _secondRowInput = _playerInput.Player.SecondRow;
        _thirdRowInput = _playerInput.Player.ThirdRow;
        _fourthRowInput = _playerInput.Player.FourthRow;

        _firstRowNotes = new List<Note>();
        _secondRowNotes = new List<Note>();
        _thirdRowNotes = new List<Note>();
        _fourthRowNotes = new List<Note>();
    }

    private void OnEnable()
    {
        _firstRowInput.performed += CheckRowInput;
        _secondRowInput.performed += CheckRowInput;
        _thirdRowInput.performed += CheckRowInput;
        _fourthRowInput.performed += CheckRowInput;

        _firstRowInput.Enable();
        _secondRowInput.Enable();
        _thirdRowInput.Enable();
        _fourthRowInput.Enable();
    }

    private void OnDisable()
    {
        _firstRowInput.performed -= CheckRowInput;
        _secondRowInput.performed -= CheckRowInput;
        _thirdRowInput.performed -= CheckRowInput;
        _fourthRowInput.performed -= CheckRowInput;

        _firstRowInput.Disable();
        _secondRowInput.Disable();
        _thirdRowInput.Disable();
        _fourthRowInput.Disable();
    }

    private void Update()
    {
        // Check for invalid notes in each row
        float currentBeat = _musicPlayer.GetTimeInBeats();
        CheckRowForInvalidNotes(_firstRowNotes, currentBeat);
        CheckRowForInvalidNotes(_secondRowNotes, currentBeat);
        CheckRowForInvalidNotes(_thirdRowNotes, currentBeat);
        CheckRowForInvalidNotes(_fourthRowNotes, currentBeat);
    }

    private void CheckRowForInvalidNotes(List<Note> _notesToCheck, float currentBeat)
    {
        if (_notesToCheck.Count <= 0)
        {
            return;
        }

        // Only check the first note in the list for each input
        Note noteToCheck = _notesToCheck[0];

        // Calculate overall note timing window
        float noteTimingWindowInBeats = _noteTimingWindowInSeconds * _musicPlayer.GetBPM() / 60;

        // Calculate upper timing window
        float noteBeat = _notesToCheck[0].beat;
        float upperTimingWindow = noteBeat + noteTimingWindowInBeats;

        // If the note is past the upper timing window
        if (currentBeat >= upperTimingWindow)
        {
            // Destroy the note object & remove it from the list
            GameObject noteObject = _notesToCheck[0].noteObject;
            Destroy(noteObject);
            _notesToCheck.RemoveAt(0);

            // Lower Score
            _uiManager.LowerScore();
        }
    }

    private void CheckRowInput(InputAction.CallbackContext context)
    {
        // Check input & call function with the correct list of notes depending on the input
        if (context.action == _firstRowInput)
        {
            CheckRowForNotes(_firstRowNotes);
            return;
        }

        if (context.action == _secondRowInput)
        {
            CheckRowForNotes(_secondRowNotes);
            return;
        }

        if (context.action == _thirdRowInput)
        {
            CheckRowForNotes(_thirdRowNotes);
            return;
        }

        if (context.action == _fourthRowInput)
        {
            CheckRowForNotes(_fourthRowNotes);
            return;
        }
    }

    private void CheckRowForNotes(List<Note> _notesToCheck)
    {
        // Lower score & return if the list is empty
        if (_notesToCheck.Count <= 0)
        {
            _uiManager.LowerScore();
            return;
        }

        // Only check the first note in the list for each input
        Note noteToCheck = _notesToCheck[0];

        // Calculate overall note timing window
        float noteTimingWindowInBeats = _noteTimingWindowInSeconds * _musicPlayer.GetBPM() / 60;

        // Calculate upper & lower timing windows
        float noteBeat = _notesToCheck[0].beat;
        float upperTimingWindow = noteBeat + noteTimingWindowInBeats;
        float lowerTimingWindow = noteBeat - noteTimingWindowInBeats;

        // Get the current beat
        float currentBeat = _musicPlayer.GetTimeInBeats();

        // If the current beat is within the timing windows
        if (currentBeat <= upperTimingWindow && currentBeat >= lowerTimingWindow)
        {
            // Destroy the note object & remove it from the list
            GameObject noteObject = _notesToCheck[0].noteObject;
            Destroy(noteObject);
            _notesToCheck.RemoveAt(0);

            // Add Score
            _uiManager.AddScore();
        }

        // Lower score if the current beat is outside the timing window
        else
        {
            _uiManager.LowerScore();
        }
    }

    public void GetNewNote(Note newNote)
    {
        // Add new note to the correct list based on its row
        switch(newNote.noteRow)
        {
            case Note.NoteRow.First:
                _firstRowNotes.Add(newNote);
                break;
            case Note.NoteRow.Second:
                _secondRowNotes.Add(newNote);
                break;
            case Note.NoteRow.Third:
                _thirdRowNotes.Add(newNote);
                break;
            case Note.NoteRow.Fourth:
                _fourthRowNotes.Add(newNote);
                break;
        }
    }
}
