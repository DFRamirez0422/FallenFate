using System.Collections.Generic;
using Dypsloom.RhythmTimeline.Core;
using Dypsloom.RhythmTimeline.Core.Input;
using Dypsloom.RhythmTimeline.Core.Managers;
using Dypsloom.RhythmTimeline.Core.Notes;
using UnityEngine;

/// <summary>
/// Auto-plays notes for a single TrackObject.
///
/// Setup:
///   1. Attach to any GameObject in your guitar battle scene.
///   2. Assign the Track Object whose notes should be auto-played.
///   3. On a TrackNoteEventReceiver (or the TrackObject itself), wire the
///      OnNoteActivate() UnityEvent to this component's AutoPlayNote() method.
///
/// When called, it reads the active notes on the TrackObject, determines the
/// type (Tap / Hold / Counter), and fires the correct input on that note's
/// own RhythmProcessor. Nothing else is touched.
/// </summary>
public class EnemyGuitarAutoPlayer : MonoBehaviour
{
    [Tooltip("The track whose notes this auto-player will hit.")]
    [SerializeField] private TrackObject m_TrackObject;

    private readonly List<HoldNote> m_ActiveHoldNotes = new List<HoldNote>();
    private readonly HashSet<Note> m_AlreadyPlayed = new HashSet<Note>();

    private RhythmDirector m_Director;
    private RhythmProcessor m_Processor;
    private int m_TrackID = -1;

    /// <summary>
    /// Wire this to OnNoteActivate() on a TrackNoteEventReceiver or TrackObject.
    /// Takes no parameters — reads active notes directly from the assigned TrackObject.
    /// </summary>
    public void AutoPlayNote()
    {
        if (m_TrackObject == null) return;

        IReadOnlyList<Note> activeNotes = m_TrackObject.Notes;
        if (activeNotes == null) return;

        for (int n = 0; n < activeNotes.Count; n++)
        {
            Note note = activeNotes[n];
            if (note == null || m_AlreadyPlayed.Contains(note)) continue;

            m_AlreadyPlayed.Add(note);

            if (!EnsureResolved(note)) continue;

            if (note is CounterNote counterNote)
            {
                int tapCount = counterNote.RhythmClipData.ClipParameters.IntParameter;
                for (int i = 0; i < tapCount; i++)
                {
                    var tap = new InputEventData(m_TrackID, 0) { Note = counterNote };
                    m_Processor.TriggerInput(tap);
                }
                continue;
            }

            var tapInput = new InputEventData(m_TrackID, 0) { Note = note };
            m_Processor.TriggerInput(tapInput);

            if (note is HoldNote holdNote)
            {
                m_ActiveHoldNotes.Add(holdNote);
            }
        }
    }

    private void Update()
    {
        if (m_Director == null || m_ActiveHoldNotes.Count == 0) return;

        float halfCrochet = m_Director.HalfCrochet;

        for (int i = m_ActiveHoldNotes.Count - 1; i >= 0; i--)
        {
            HoldNote hold = m_ActiveHoldNotes[i];

            if (hold == null || !hold.gameObject.activeSelf)
            {
                m_ActiveHoldNotes.RemoveAt(i);
                continue;
            }

            if (hold.TimeFromDeactivate + halfCrochet > 0)
            {
                var release = new InputEventData(hold.RhythmClipData.TrackID, 1) { Note = hold };
                m_Processor.TriggerInput(release);
                m_ActiveHoldNotes.RemoveAt(i);
            }
        }

        m_AlreadyPlayed.RemoveWhere(n => n == null || !n.gameObject.activeSelf);
    }

    private bool EnsureResolved(Note note)
    {
        if (m_Processor != null) return true;

        RhythmDirector dir = note.RhythmClipData.RhythmDirector;
        if (dir == null) return false;

        TrackObject[] tracks = dir.TrackObjects;
        for (int i = 0; i < tracks.Length; i++)
        {
            if (tracks[i] == m_TrackObject)
            {
                m_TrackID = i;
                break;
            }
        }

        if (m_TrackID == -1)
        {
            Debug.LogError(
                $"[EnemyGuitarAutoPlayer] TrackObject '{m_TrackObject.name}' not found on RhythmDirector '{dir.gameObject.name}'.",
                this);
            return false;
        }

        m_Director = dir;
        m_Processor = dir.RhythmProcessor;
        return true;
    }
}
