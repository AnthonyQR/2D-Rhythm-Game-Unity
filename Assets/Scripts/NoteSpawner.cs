using UnityEngine;
using System;
using System.Collections.Generic;

public class NoteSpawner : MonoBehaviour
{
    // Text asset to read notes from
    [SerializeField] private TextAsset _beatmap;

    // Reference music player to sync note spawns with the music
    [SerializeField] private MusicPlayer _musicPlayer;
    [SerializeField] private NoteScroller _noteScroller;
    [SerializeField] private NoteJudger _noteJudger;

    // Number of beats before the note's beat to spawn music
    [SerializeField] private float _noteSpawnBeatOffset;

    // Rows to spawn notes in
    [Header("Rows")]
    [SerializeField] private Transform _firstRow;
    [SerializeField] private Transform _secondRow;
    [SerializeField] private Transform _thirdRow;
    [SerializeField] private Transform _fourthRow;

    // Prefabs for each note to be spawned
    [Header("Note Prefabs")]
    [SerializeField] private GameObject _firstRowNotePrefab;
    [SerializeField] private GameObject _secondRowNotePrefab;
    [SerializeField] private GameObject _thirdRowNotePrefab;
    [SerializeField] private GameObject _fourthRowNotePrefab;

    // List of notes to spawn as the track plays
    private List<Note> _notes;

    void Start()
    {
        _notes = new List<Note>();
        ReadBeatmap();
        CheckNotesToSpawn();
    }

    private void Update()
    {
        CheckNotesToSpawn();
    }

    private void ReadBeatmap()
    {
        // Split each line in file into a separate element
        string[] data = _beatmap.text.Split(new string[] { "\n" }, StringSplitOptions.None);
        foreach (string s in data)
        {
            // Setup note data
            string[] noteData = null;
            Note.NoteRow newNoteRow = new Note.NoteRow();
            float newBeat = 0f;
            bool newIsHold = false;
            float newHoldDuration = 0f;
            GameObject newNoteObject = null;


            // Try to split line with commas
            // Skip line if not in csv format
            try
            {
                noteData = s.Split(",");
            }
            catch 
            {
                Debug.Log("Skipping line due to not being in a csv format");
                continue;
            }


            // Try to get note row & beat information from line
            // Skip line if note row or beat information is incorrect
            try
            {
                bool correctNoteRow = Enum.TryParse(noteData[0], out newNoteRow);
                if (!correctNoteRow)
                {
                    continue;
                }

                bool correctBeat = float.TryParse(noteData[1], out newBeat);
                if (!correctBeat)
                {
                    continue;
                }
            }
            catch
            {
                Debug.Log("Skipping line due to not finding note row or beat information");
                continue;
            }
            

            // Try to get hold note information from line
            // Set default values if the information is incorrect
            try
            {
                bool correctIsHold = bool.TryParse(noteData[2], out newIsHold);
                if (!correctIsHold)
                {
                    newIsHold = false;
                }

                bool correctHoldDuration = float.TryParse(noteData[3], out newHoldDuration);
                if (!correctHoldDuration)
                {
                    newHoldDuration = 0f;
                }
            }
            catch
            {
                Debug.Log("Set default values for hold notes since they are missing / incorrect");
                newIsHold = false;
                newHoldDuration = 0f;
            }
            

            // Create new note & add to the array if there are no errors
            Note newNote = new Note(newNoteRow, newBeat, newIsHold, newHoldDuration, newNoteObject);
            _notes.Add(newNote);
            Debug.Log("New Note Added");
        }
    }

    private void CheckNotesToSpawn()
    {
        // Return immediately if no new notes can be spawned
        if (_notes.Count <= 0)
        {
            return;
        }

        // While the note's beat - offset to spawn the note early
        // is less than the current beat
        while (_notes[0].beat - _noteSpawnBeatOffset <= _musicPlayer.GetTimeInBeats())
        {
            // Spawn the note & remove it
            SpawnNote(_notes[0]);
            _notes.RemoveAt(0);

            // Return if there are no more notes so an error doesn't occur
            if (_notes.Count <= 0)
            {
                return;
            }
        }
    }

    private void SpawnNote(Note note)
    {
        // Null object to reference later
        GameObject newNote = null;

        // Calculate beat to spawn the note at
        float noteBeatOffset = _musicPlayer.GetInitialDelayInBeats();
        float beatToSpawn = note.beat + noteBeatOffset;

        // Calculate position based on scroll speed
        float scrollSpeedPosition = _noteScroller.GetScrollSpeed() * beatToSpawn;

        // Get bps for position calculations
        float beatsPerSecond = _musicPlayer.GetBPM() / 60;

        // Calculate position based on scroll speed AND song bps
        float newNotePosition = scrollSpeedPosition / beatsPerSecond;

        // Create Vector3 to move note after instantiating
        Vector3 totalNotePosition = new Vector3(newNotePosition, 0f, 0f);

        // Spawn correct note in the correct row based on Enum
        switch(note.noteRow)
        {
            case Note.NoteRow.First:
                newNote = Instantiate(_firstRowNotePrefab, _firstRow.position, _firstRowNotePrefab.transform.rotation, _firstRow);
                break;
            case Note.NoteRow.Second:
                newNote = Instantiate(_secondRowNotePrefab, _secondRow.position, _secondRowNotePrefab.transform.rotation, _secondRow);
                break;
            case Note.NoteRow.Third:
                newNote = Instantiate(_thirdRowNotePrefab, _thirdRow.position, _thirdRowNotePrefab.transform.rotation, _thirdRow);
                break;
            case Note.NoteRow.Fourth:
                newNote = Instantiate(_fourthRowNotePrefab, _fourthRow.position, _fourthRowNotePrefab.transform.rotation, _fourthRow);
                break;
        }

        // Move note to the correct position
        newNote.transform.localPosition = totalNotePosition;

        // Hand note over to Note Judger
        note.noteObject = newNote;
        _noteJudger.GetNewNote(note);
    }
}
