using UnityEngine;

namespace NPA_RhythmBonusPrefabs
{
    /// <summary>
    /// Judges attack timing against the music grid
    /// Call EvaluateNow() exactly when you commit the attack (input or damage frame)
    /// </summary>
    public class RhythmBonusJudge : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RhythmMusicPlayer music; // Reference to music/clock script

        [Header("Enabled Grids")] [SerializeField]
        [Tooltip("Valid subdivisions per quarter note: 1=quarters, 2=eighths, 4=sixteenths")]
        private float[] stepsOptions = new float[] { 1f, 2f };

        [Header("Timing Windows (base at quarter notes)")] 
        [SerializeField] private float perfectQuarterSec = 0.08f; // Perfect window (80 ms)
        [SerializeField] private float goodQuarterSec = 0.15f;    // Good window (150 ms)
        
        [Header("Multiplier Bonuses")]
        [SerializeField] private float perfectMult = 1.50f;
        [SerializeField] private float goodMult = 1.25f;
        [SerializeField] private float missMult = 1.00f;
        
        [Header("Calibration")]
        [Tooltip("Positive = judge later; Negative = earlier")]
        [SerializeField] private float latencyOffsetSec = 0f;   // Offset to compensate for device/controller lag

        // Enumeration to be returned by EvaluateNow() to determine how well the beat was hit to the rhythm.
        // If there are more multiplier bonuses planned, PLEASE write them in the enum to ensure they are
        // evluated correctly!
        public enum RhythmTier
        {
            Perfect,
            Good,
            Miss
        }

        public (RhythmTier tier, float multiplier) EvaluateNow()
        {
            if (music == null) return (RhythmTier.Miss, missMult);
            if (!music.IsPlaying) return (RhythmTier.Miss, missMult);
            
            // Get current elapsed song time in seconds (DSP-accurate) 
            double elapsedSec = music.GetElapsedSec() + latencyOffsetSec;
            
            float bestDistSec = float.MaxValue; // Closest to any grid
            float bestIntervalSec = 0f;         // the interval that won

            foreach (var option in stepsOptions)
            {
                float steps = Mathf.Max(1f, option); // Avoid 0/negative
                float intervalSec = music.BeatSec / steps;    // Note length for this grid
                double idx = elapsedSec / intervalSec;        // Fractional index into that grid 
                double nearestIdx = System.Math.Round(idx);   // Nearest gridline
                float distSec = (float)(System.Math.Abs(      // Distance (sec)
                    idx - nearestIdx) * intervalSec);

                if (distSec < bestDistSec)
                {
                    bestDistSec = distSec;
                    bestIntervalSec = intervalSec; // Remember which grid we matched
                }
            }

            // Scale timing windows to note length
            float perfect = perfectQuarterSec * (bestIntervalSec / music.BeatSec);
            float good    = goodQuarterSec    * (bestIntervalSec / music.BeatSec);
            
            // Compare distance to thresholds → return tier + multiplier
            if (bestDistSec <= perfect) return (RhythmTier.Perfect, perfectMult);
            if (bestDistSec <= good)    return (RhythmTier.Good,    goodMult);
            return (RhythmTier.Miss, missMult);
        }
    }
}