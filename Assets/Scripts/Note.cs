using UnityEngine;

public struct Note
{
    // Information for each note that is spawned in
    public enum NoteRow
    {
        First,
        Second,
        Third,
        Fourth
    }

    public NoteRow noteRow;
    public float beat;
    public bool isHold;
    public float holdDuration;
    public GameObject noteObject;

    public HoldNoteObject holdNoteScript;
    public int pointsScored;

    public Note(NoteRow newNoteRow, float newBeat, bool newIsHold, float newHoldDuration, 
        GameObject newNoteObject)
    {
        noteRow = newNoteRow;
        beat = newBeat;
        isHold = newIsHold;
        holdDuration = newHoldDuration;
        noteObject = newNoteObject;

        holdNoteScript = null;
        pointsScored = 0;
    }
}
