using System.Collections.Generic;
using Dypsloom.RhythmTimeline.Core;
using Dypsloom.RhythmTimeline.Core.Input;
using Dypsloom.RhythmTimeline.Core.Managers;
using Dypsloom.RhythmTimeline.Core.Notes;
using Dypsloom.Shared;
using UnityEngine;

/// <summary>
/// Auto-plays an enemy guitar AI track by registering perfect note hits directly on the
/// RhythmProcessor, bypassing RhythmInputManager and any key bindings entirely.
///
/// How to use:
///   1. Attach this script to any GameObject in your rhythm battle scene.
///   2. Assign the Rhythm Director (or leave empty to auto-find via PlayerID Toolbox).
///   3. Assign the Track Object that belongs to the enemy's guitar track.
///
/// Supported note types:
///   TapNote     — fires a single Tap on activate.
///   HoldNote    — fires Tap on activate, then Release at the perfect release window each Update.
///   CounterNote — fires N Taps on activate (N = the clip's IntParameter counter value),
///                 draining the counter instantly for a perfect score.
/// </summary>
public class EnemyGuitarAutoPlayer : MonoBehaviour
{
    [Tooltip("Must match the PlayerID set on the Rhythm Director and other rhythm components.")]
    [SerializeField] private uint m_PlayerID = 1;

    [Tooltip("The Rhythm Director GameObject. Leave empty to auto-find via Toolbox using PlayerID.")]
    [SerializeField] private RhythmDirector m_RhythmDirector;

    [Tooltip("The Track Object the enemy AI should auto-hit. Overrides Track ID when assigned.")]
    [SerializeField] private TrackObject m_TrackObject;

    [Tooltip("Fallback track index if no Track Object is assigned. -1 means all tracks.")]
    [SerializeField] private int m_TrackID = -1;

    private RhythmProcessor m_RhythmProcessor;

    // HoldNotes currently being held — polled each Update to release at the perfect window.
    private readonly List<HoldNote> m_ActiveHoldNotes = new List<HoldNote>();

    private void Start()
    {
        if (m_RhythmDirector == null)
        {
            m_RhythmDirector = Toolbox.Get<RhythmDirector>(m_PlayerID);
        }

        m_RhythmProcessor = m_RhythmDirector.RhythmProcessor;

        // Resolve numeric TrackID from the TrackObject reference so we can filter events.
        if (m_TrackObject != null)
        {
            TrackObject[] tracks = m_RhythmDirector.TrackObjects;
            for (int i = 0; i < tracks.Length; i++)
            {
                if (tracks[i] == m_TrackObject)
                {
                    m_TrackID = i;
                    break;
                }
            }
        }

        m_RhythmProcessor.OnNoteActivateEvent += HandleNoteActivate;
    }

    private void Update()
    {
        // Mirror HoldNote's m_AutoPerfectRelease logic from outside:
        // release when (TimeFromDeactivate + HalfCrochet) crosses zero.
        float halfCrochet = m_RhythmDirector.HalfCrochet;

        for (int i = m_ActiveHoldNotes.Count - 1; i >= 0; i--)
        {
            HoldNote holdNote = m_ActiveHoldNotes[i];

            // Guard against notes that were missed/destroyed externally.
            if (holdNote == null || !holdNote.gameObject.activeSelf)
            {
                m_ActiveHoldNotes.RemoveAt(i);
                continue;
            }

            if (holdNote.TimeFromDeactivate + halfCrochet > 0)
            {
                var releaseInput = new InputEventData(holdNote.RhythmClipData.TrackID, 1);
                releaseInput.Note = holdNote;
                m_RhythmProcessor.TriggerInput(releaseInput);
                m_ActiveHoldNotes.RemoveAt(i);
            }
        }
    }

    private void HandleNoteActivate(Note note)
    {
        // Filter to the enemy's specific track.
        if (m_TrackID != -1 && note.RhythmClipData.TrackID != m_TrackID) { return; }

        if (note is CounterNote)
        {
            // Burst-fire exactly N taps (the clip's IntParameter) to drain the counter to zero
            // for a perfect score. Skip the single tap below — this loop covers all of them.
            int tapCount = note.RhythmClipData.ClipParameters.IntParameter;
            for (int i = 0; i < tapCount; i++)
            {
                var counterTap = new InputEventData(note.RhythmClipData.TrackID, 0);
                counterTap.Note = note;
                m_RhythmProcessor.TriggerInput(counterTap);
            }
            return;
        }

        // TapNote / HoldNote: send a single Tap (InputID 0) directly to the processor.
        var tapInput = new InputEventData(note.RhythmClipData.TrackID, 0);
        tapInput.Note = note;
        m_RhythmProcessor.TriggerInput(tapInput);

        // Queue HoldNotes so Update() can release them at the perfect window.
        if (note is HoldNote holdNote)
        {
            m_ActiveHoldNotes.Add(holdNote);
        }
    }

    private void OnDestroy()
    {
        if (m_RhythmProcessor != null)
        {
            m_RhythmProcessor.OnNoteActivateEvent -= HandleNoteActivate;
        }
    }
}
