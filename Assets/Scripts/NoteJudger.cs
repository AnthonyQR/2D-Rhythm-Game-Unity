using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoteJudger : MonoBehaviour
{
    // Float to determine the timing window for note to be valid
    [SerializeField] private float _noteTimingWindowInSeconds;

    // Referece components for calculation judgement area
    [SerializeField] private MusicPlayer _musicPlayer;

    [SerializeField] private ScoreManager _scoreManager;

    // List of notes for each row to check individually
    private List<Note> _firstRowNotes;
    private List<Note> _secondRowNotes;
    private List<Note> _thirdRowNotes;
    private List<Note> _fourthRowNotes;

    // Separate lists for hold notes
    private List<Note> _firstRowHoldNotes;
    private List<Note> _secondRowHoldNotes;
    private List<Note> _thirdRowHoldNotes;
    private List<Note> _fourthRowHoldNotes;

    // Input actions for each row
    private PlayerInput _playerInput;
    private InputAction _firstRowInput;
    private InputAction _secondRowInput;
    private InputAction _thirdRowInput;
    private InputAction _fourthRowInput;

    // Separate actions for hold notes
    private InputAction _firstRowHoldInput;
    private InputAction _secondRowHoldInput;
    private InputAction _thirdRowHoldInput;
    private InputAction _fourthRowHoldInput;

    // Check for continuously holding notes
    private bool _isFirstRowHold = false;
    private bool _isSecondRowHold = false;
    private bool _isThirdRowHold = false;
    private bool _isFourthRowHold = false;

    private void Awake()
    {
        _playerInput = new PlayerInput();

        _firstRowInput = _playerInput.Player.FirstRow;
        _secondRowInput = _playerInput.Player.SecondRow;
        _thirdRowInput = _playerInput.Player.ThirdRow;
        _fourthRowInput = _playerInput.Player.FourthRow;

        _firstRowHoldInput = _playerInput.Player.FirstRowHold;
        _secondRowHoldInput = _playerInput.Player.SecondRowHold;
        _thirdRowHoldInput = _playerInput.Player.ThirdRowHold;
        _fourthRowHoldInput = _playerInput.Player.FourthRowHold;

        _firstRowNotes = new List<Note>();
        _secondRowNotes = new List<Note>();
        _thirdRowNotes = new List<Note>();
        _fourthRowNotes = new List<Note>();

        _firstRowHoldNotes = new List<Note>();
        _secondRowHoldNotes = new List<Note>();
        _thirdRowHoldNotes = new List<Note>();
        _fourthRowHoldNotes = new List<Note>();
    }

    private void OnEnable()
    {
        _firstRowInput.performed += CheckNoteRowInput;
        _secondRowInput.performed += CheckNoteRowInput;
        _thirdRowInput.performed += CheckNoteRowInput;
        _fourthRowInput.performed += CheckNoteRowInput;

        _firstRowHoldInput.performed += CheckHoldNoteRowInput;
        _secondRowHoldInput.performed += CheckHoldNoteRowInput;
        _thirdRowHoldInput.performed += CheckHoldNoteRowInput;
        _fourthRowHoldInput.performed += CheckHoldNoteRowInput;

        _firstRowHoldInput.canceled += CheckCancelledHoldNoteInput;
        _secondRowHoldInput.canceled += CheckCancelledHoldNoteInput;
        _thirdRowHoldInput.canceled += CheckCancelledHoldNoteInput;
        _fourthRowHoldInput.canceled += CheckCancelledHoldNoteInput;

        _firstRowInput.Enable();
        _secondRowInput.Enable();
        _thirdRowInput.Enable();
        _fourthRowInput.Enable();

        _firstRowHoldInput.Enable();
        _secondRowHoldInput.Enable();
        _thirdRowHoldInput.Enable();
        _fourthRowHoldInput.Enable();
    }

    private void OnDisable()
    {
        _firstRowInput.performed -= CheckNoteRowInput;
        _secondRowInput.performed -= CheckNoteRowInput;
        _thirdRowInput.performed -= CheckNoteRowInput;
        _fourthRowInput.performed -= CheckNoteRowInput;

        _firstRowHoldInput.performed -= CheckHoldNoteRowInput;
        _secondRowHoldInput.performed -= CheckHoldNoteRowInput;
        _thirdRowHoldInput.performed -= CheckHoldNoteRowInput;
        _fourthRowHoldInput.performed -= CheckHoldNoteRowInput;

        _firstRowHoldInput.canceled -= CheckCancelledHoldNoteInput;
        _secondRowHoldInput.canceled -= CheckCancelledHoldNoteInput;
        _thirdRowHoldInput.canceled -= CheckCancelledHoldNoteInput;
        _fourthRowHoldInput.canceled -= CheckCancelledHoldNoteInput;

        _firstRowInput.Disable();
        _secondRowInput.Disable();
        _thirdRowInput.Disable();
        _fourthRowInput.Disable();

        _firstRowHoldInput.Disable();
        _secondRowHoldInput.Disable();
        _thirdRowHoldInput.Disable();
        _fourthRowHoldInput.Disable();
    }

    private void Update()
    {
        // Check for invalid notes in each row
        float currentBeat = _musicPlayer.GetTimeInBeats();
        CheckRowForInvalidNotes(_firstRowNotes, currentBeat, false);
        CheckRowForInvalidNotes(_secondRowNotes, currentBeat, false);
        CheckRowForInvalidNotes(_thirdRowNotes, currentBeat, false);
        CheckRowForInvalidNotes(_fourthRowNotes, currentBeat, false);

        CheckRowForInvalidNotes(_firstRowHoldNotes, currentBeat, _isFirstRowHold);
        CheckRowForInvalidNotes(_secondRowHoldNotes, currentBeat, _isSecondRowHold);
        CheckRowForInvalidNotes(_thirdRowHoldNotes, currentBeat, _isThirdRowHold);
        CheckRowForInvalidNotes(_fourthRowHoldNotes, currentBeat, _isFourthRowHold);

        CheckContinuousHoldNoteInput();
    }

    private void CheckRowForInvalidNotes(List<Note> _notesToCheck, float currentBeat, bool heldNote)
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
            if (!noteToCheck.isHold)
            {
                // Destroy the note object & remove it from the list
                GameObject noteObject = _notesToCheck[0].noteObject;
                Destroy(noteObject);
                
            }

            else if (heldNote)
            {
                return;
            }

            else
            {
                noteToCheck.holdNoteScript.StartDestroy();
            }

            _notesToCheck.RemoveAt(0);
            // Lower Score
            _scoreManager.LowerScore(true);
        }
    }

    private void CheckNoteRowInput(InputAction.CallbackContext context)
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
            _scoreManager.LowerScore();
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
            _scoreManager.AddScore(true);
        }

        // Lower score if the current beat is outside the timing window
        else
        {
            _scoreManager.LowerScore();
        }
    }

    private void CheckHoldNoteRowInput(InputAction.CallbackContext context)
    {
        if (context.action == _firstRowHoldInput)
        {
            _isFirstRowHold = CheckRowForHoldNotes(_firstRowHoldNotes);
            return;
        }

        if (context.action == _secondRowHoldInput)
        {
            _isSecondRowHold = CheckRowForHoldNotes(_secondRowHoldNotes);
            return;
        }

        if (context.action == _thirdRowHoldInput)
        {
            _isThirdRowHold = CheckRowForHoldNotes(_thirdRowHoldNotes);
            return;
        }

        if (context.action == _fourthRowHoldInput)
        {
            _isFourthRowHold = CheckRowForHoldNotes(_fourthRowHoldNotes);
            return;
        }
    }
    private bool CheckRowForHoldNotes(List<Note> _notesToCheck)
    {
        // Lower score & return if the list is empty
        if (_notesToCheck.Count <= 0)
        {
            _scoreManager.LowerScore();
            return false;
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
            // Add Score
            _scoreManager.AddScore(true);
            return true;
        }

        // Lower score if the current beat is outside the timing window
        else
        {
            _scoreManager.LowerScore();
            return false;
        }
    }

    private void CheckContinuousHoldNoteInput()
    {
        if (_isFirstRowHold)
        {
            _isFirstRowHold = CheckRowForContinuousHoldNote(_firstRowHoldNotes);
        }

        if (_isSecondRowHold)
        {
            _isSecondRowHold = CheckRowForContinuousHoldNote(_secondRowHoldNotes);
        }

        if (_isThirdRowHold)
        {
            _isThirdRowHold = CheckRowForContinuousHoldNote(_thirdRowHoldNotes);
        }

        if (_isFourthRowHold)
        {
            _isFourthRowHold = CheckRowForContinuousHoldNote(_fourthRowHoldNotes);
        }
    }

    private bool CheckRowForContinuousHoldNote(List<Note> _notesToCheck)
    {
        // Only check the first note in the list for each input
        Note noteToCheck = _notesToCheck[0];

        noteToCheck.holdNoteScript.UpdateHoldNoteVisuals();
        noteToCheck.pointsScored = noteToCheck.holdNoteScript.GetPointsScored();

        // Get the current beat
        float currentBeat = _musicPlayer.GetTimeInBeats();
        float noteHoldFinalBeat = noteToCheck.beat + noteToCheck.holdDuration;

        // If the note has been held for long enough
        if (currentBeat >= noteHoldFinalBeat)
        {
            // Add maximum hold note score & return false to stop checking for hold notes
            _scoreManager.AddScoreHoldNote(noteToCheck, true);
            noteToCheck.holdNoteScript.StartDestroy();
            _notesToCheck.RemoveAt(0);
            return false;
        }

        else
        {
            // Add score
            // Note is still being held down
            int pointsScored = _scoreManager.AddScoreHoldNote(noteToCheck);
            noteToCheck.holdNoteScript.UpdatePointsScored(pointsScored);
            return true;
        }   
    }

    private void CheckCancelledHoldNoteInput(InputAction.CallbackContext context) 
    {
        if (context.action == _firstRowHoldInput && _isFirstRowHold)
        {
            CheckRowForCancelledHoldNote(_firstRowHoldNotes);
            _isFirstRowHold = false;
        }

        if (context.action == _secondRowHoldInput && _isSecondRowHold)
        {
            CheckRowForCancelledHoldNote(_secondRowHoldNotes);
            _isSecondRowHold = false;
        }

        if (context.action == _thirdRowHoldInput && _isThirdRowHold)
        {
            CheckRowForCancelledHoldNote(_thirdRowHoldNotes);
            _isThirdRowHold = false;
        }

        if (context.action == _fourthRowHoldInput && _isFourthRowHold)
        {
            CheckRowForCancelledHoldNote(_fourthRowHoldNotes);
            _isFourthRowHold = false;
        }
    }

    private void CheckRowForCancelledHoldNote(List<Note> _notesToCheck)
    {
        // Only check the first note in the list for each input
        Note noteToCheck = _notesToCheck[0];

        // Get the current beat
        float currentBeat = _musicPlayer.GetTimeInBeats();
        float noteHoldFinalBeat = noteToCheck.beat + noteToCheck.holdDuration;

        // Calculate overall note timing window
        float noteTimingWindowInBeats = _noteTimingWindowInSeconds * _musicPlayer.GetBPM() / 60;

        // Calculate lower timing windows
        float lowerTimingWindow = noteHoldFinalBeat - noteTimingWindowInBeats;

        // Allow to cancel slightly early
        if (currentBeat >= lowerTimingWindow)
        {
            // Add maximum hold note score
            _scoreManager.AddScoreHoldNote(noteToCheck, true);
        }

        else
        {
            _scoreManager.LowerScore();
        }
        noteToCheck.holdNoteScript.StartDestroy();
        _notesToCheck.RemoveAt(0);
    }
    

    public void GetNewNote(Note newNote)
    {
        // Add new note to the correct list based on its row & note type

        if (!newNote.isHold)
        {
            switch (newNote.noteRow)
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

        else
        {
            switch (newNote.noteRow)
            {
                case Note.NoteRow.First:
                    _firstRowHoldNotes.Add(newNote);
                    break;
                case Note.NoteRow.Second:
                    _secondRowHoldNotes.Add(newNote);
                    break;
                case Note.NoteRow.Third:
                    _thirdRowHoldNotes.Add(newNote);
                    break;
                case Note.NoteRow.Fourth:
                    _fourthRowHoldNotes.Add(newNote);
                    break;
            }
        }
    }
}
