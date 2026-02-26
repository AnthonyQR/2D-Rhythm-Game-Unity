using TMPro;
using Unity.Collections;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private MusicPlayer _musicPlayer;
    [SerializeField] private int _baseNoteScore;
    private int _score;
    private int _notesHit;
    private int _notesTotal;
    private int _mistakes;

    private int[] _multipliers = { 1, 2, 3, 4, 5, 10 };
    private int _currentMultiplierLevel;

    private int _comboCount;
    private int _comboMultiplierRequirement = 10;

    private void Awake()
    {
        _score = 0;
        _notesHit = 0;
        _notesTotal = 0;
        _mistakes = 0;
    }

    public void AddScore(bool updateNote = false)
    {
        DetermineMultiplier(true);
        _score += _baseNoteScore * _multipliers[_currentMultiplierLevel];

        if (updateNote)
        {
            _notesHit++;
            _notesTotal++;
        }
        CallUpdateScore();
    }

    public void LowerScore(bool updateNote = false)
    {
        DetermineMultiplier(false);
        _score -= _baseNoteScore;
        _mistakes++;

        if (updateNote)
        {
            _notesTotal++;
        }
        CallUpdateScore();
    }

    public int AddScoreHoldNote(Note holdNote, bool isHoldNoteEnd = false)
    {
        float holdNoteInitialBeat = holdNote.beat;
        float holdNoteBeatDuration = holdNote.holdDuration;
        float currentBeat = _musicPlayer.GetTimeInBeats();
        int pointsAlreadyScored = holdNote.pointsScored;

        float beatsNoteIsHeld = currentBeat - holdNoteInitialBeat;
        float secondsNoteIsHeld = beatsNoteIsHeld / _musicPlayer.GetBPM() * 60;
        int totalScoreToBeAdded = 0;

        if (!isHoldNoteEnd)
        {
            totalScoreToBeAdded = (int)(secondsNoteIsHeld * _baseNoteScore * _multipliers[_currentMultiplierLevel]);
            Debug.Log(pointsAlreadyScored);
            _score += (totalScoreToBeAdded - pointsAlreadyScored);
        }
        else
        {
            float holdNoteSecondsDuration = holdNoteBeatDuration / _musicPlayer.GetBPM() / 60;
            totalScoreToBeAdded = (int)(holdNoteBeatDuration * _baseNoteScore * _multipliers[_currentMultiplierLevel]);
            _score += (totalScoreToBeAdded);
            _score -= pointsAlreadyScored;
        }

        CallUpdateScore();
        return totalScoreToBeAdded;
    }

    private void DetermineMultiplier(bool isNoteHit)
    {
        // If a note was hit
        if (isNoteHit)
        {
            // Return if combo multiplier is at max level
            if (_currentMultiplierLevel == _multipliers.Length - 1)
            {
                return;
            }

            // Else, increase the combo count
            _comboCount++;

            // Reset combo count & increase multiplier level if combo count is high enough
            if (_comboCount >= _comboMultiplierRequirement)
            {
                _comboCount = 0;
                _currentMultiplierLevel++;
            }
        }

        // If note was missed, or a misspress occurred
        else
        {
            // Return if multiplier level & combo count are both at 0
            if (_currentMultiplierLevel == 0 && _comboCount == 0)
            {
                return;
            }

            // Reset combo count if it's not 0
            if (_comboCount != 0)
            {
                _comboCount = 0;
            }
            
            // Else, maximize combo count & lower multiplier level
            else
            {
                _comboCount = _comboMultiplierRequirement - 1;
                _currentMultiplierLevel--;
            }
        }
    }

    private void CallUpdateScore()
    {
        _uiManager.UpdateScoreText(_score, _notesHit, _notesTotal, _mistakes);
        _uiManager.UpdateCombo(_comboCount, _multipliers[_currentMultiplierLevel]);
    }
}
