using System;
using UnityEngine;

namespace Rhythm
{
    public enum HitTier { Miss, Good, Perfect }

    /// <summary>
    /// Feed it a sorted array of note times (in seconds).
    /// Call EvaluateHit(currentSongTimeSec) exactly when the player commits the hit.
    /// </summary>
    public class ChartTimingJudge : MonoBehaviour
    {
        [Header("Windows (in seconds) at note-level")]
        [Tooltip("Max absolute timing error for a Perfect.")]
        public float perfectWindow = 0.040f; // 40 ms

        [Tooltip("Max absolute timing error for a Good (above Perfect).")]
        public float goodWindow = 0.100f;    // 100 ms

        [Header("Calibration")]
        [Tooltip("Positive = judge later, Negative = judge earlier (seconds).")]
        public float latencyOffsetSec = 0f;

        // ----- Runtime state -----
        private double[] _noteTimes = Array.Empty<double>();
        private int _nextIdx = 0;

        /// <summary>Initialize/replace the note times (must be sorted ascending, in seconds).</summary>
        public void SetNotes(double[] noteTimesSorted)
        {
            _noteTimes = noteTimesSorted ?? Array.Empty<double>();
            _nextIdx = 0;
        }

        /// <summary>
        /// Judge a hit occurring at the given song time (in seconds, DSP-accurate if possible).
        /// Returns the tier, the signed error (seconds; negative = early, positive = late),
        /// and the index of the judged note (or -1 on Miss with no candidate).
        /// </summary>
        public (HitTier tier, float errorSec, int noteIndex) EvaluateHit(double currentSongTimeSec)
        {
            if (_noteTimes.Length == 0) return (HitTier.Miss, 0f, -1);
            if (_nextIdx >= _noteTimes.Length) return (HitTier.Miss, 0f, -1);

            // Apply calibration offset so we judge against a shifted reference
            double judgedTime = currentSongTimeSec + latencyOffsetSec;

            // 1) Drop any notes that are way in the past beyond the Good window (can’t be hit anymore)
            while (_nextIdx < _noteTimes.Length &&
                   judgedTime - _noteTimes[_nextIdx] > goodWindow)
            {
                _nextIdx++;
            }
            if (_nextIdx >= _noteTimes.Length) return (HitTier.Miss, 0f, -1);

            // 2) Find the nearest candidate among next few notes (usually next is enough)
            int bestIdx = _nextIdx;
            double bestAbsErr = Math.Abs(_noteTimes[bestIdx] - judgedTime);

            // Check the immediate next note too (useful if input is slightly late/early around a dense section)
            int lookAhead = Math.Min(_nextIdx + 2, _noteTimes.Length - 1);
            for (int i = _nextIdx + 1; i <= lookAhead; i++)
            {
                double err = Math.Abs(_noteTimes[i] - judgedTime);
                if (err < bestAbsErr)
                {
                    bestAbsErr = err;
                    bestIdx = i;
                }
            }

            float signedErr = (float)(judgedTime - _noteTimes[bestIdx]); // +late / -early

            // 3) Compare against windows
            if (bestAbsErr <= perfectWindow)
            {
                // consume this note
                _nextIdx = bestIdx + 1;
                return (HitTier.Perfect, signedErr, bestIdx);
            }
            if (bestAbsErr <= goodWindow)
            {
                _nextIdx = bestIdx + 1;
                return (HitTier.Good, signedErr, bestIdx);
            }

            // Not within Good: Miss (do not consume any note; player can still attempt the same upcoming note)
            return (HitTier.Miss, signedErr, -1);
        }

        // Convenience: evaluate using Time.time (less precise than DSP time; prefer AudioSettings.dspTime if available)
        public (HitTier tier, float errorSec, int noteIndex) EvaluateHitNow(double songStartTimeSec)
        {
            double t = Time.timeAsDouble - songStartTimeSec;
            return EvaluateHit(t);
        }
    }
}
