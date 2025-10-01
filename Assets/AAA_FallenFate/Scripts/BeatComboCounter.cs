using UnityEngine;

namespace NPA_RhythmBonusPrefabs
{
    using RhythmTier =RhythmBonusJudge.RhythmTier;

    public class BeatComboCounter : MonoBehaviour
    {
        /*
            DESCRIPTION: BeatComboCounter
            AUTHOR: Jose Escobedo

            Code modified from the original RhythmJudgeTester.cs file created by:
                MIKE HERNANDEZ

            Helper script that counts the combos, number of perfect hits and misses, the overall
            timing grade for the player, and evaluates how well did the player hit a beat on rhythm.
        */

        [Tooltip("Reference to the rhythm evaluation system deemed the bonus judge.")]
        [SerializeField] private RhythmBonusJudge m_Judge;

        // Counts how many beats were hit in the current combo.
        private int m_ComboCounter = 0;

        // Counts the highest combo ever recorded.
        private int m_MaxCombos = 0;

        // Counts how many total beats were hit perfectly.
        private int m_NumPerfectHits = 0;

        // Counts how many total beats were missed in rhythm.
        private int m_NumMisses = 0;

        // Main function to be called to evaluate whether or not the music was hit on beat and updates
        // the counters accordingly. In other words, this is intended to be called by external code
        // to decide whether or not the player hit on beat and automatically counts up the combo
        // counter.
        //
        // Returns the tier - whether or not a perfect hit was done on beat or not.
        public RhythmTier EvaluateBeat()
        {
            var (tier, mult) = m_Judge.EvaluateNow(); // Call the judge

            switch (tier)
            {
                case RhythmTier.Perfect:
                    m_NumPerfectHits += 1;
                    m_ComboCounter += 1;
                    break;
                case RhythmTier.Good:
                case RhythmTier.Miss:
                default:
                    m_NumMisses += 1;
                    m_ComboCounter = 0;
                    break;
            }

            if (m_ComboCounter > m_MaxCombos)
            {
                m_MaxCombos = m_ComboCounter;
            }

            return tier;
        }

        // Reset all internal counters. Can be used if outside code wants a clean slate with combos and the number
        // of total hits and misses.
        public void Reset()
        {
            m_ComboCounter = 0;
            m_MaxCombos = 0;
            m_NumPerfectHits = 0;
            m_NumMisses = 0;
        }

        // Reset the combo counter by force. Can be used if outside code needs to reset a combo on purpose.
        public void ResetCombo()
        {
            m_ComboCounter = 0;
            m_MaxCombos = 0;
        }

        // Returns how many perfect hits were done on combo.
        public int GetCurrentCombo()
        {
            return m_ComboCounter;
        }

        // Returns the highest combo recorded.
        public int GetMaxEverCombo()
        {
            return m_MaxCombos;
        }

        // Returns how many perfect hits were done in total.
        public int GetNumberOfPerfects()
        {
            return m_NumPerfectHits;
        }

        // Returns how many misses were done in total.
        public int GetNumberOfMisses()
        {
            return m_NumMisses;
        }

        // Returns how many perfect hits were done as a percentage between zero and one.
        public float GetTimingGrade()
        {
            // Special case to avoid a division by zero.
            if (m_NumPerfectHits == 0 || m_NumMisses == 0)
            {
                return 1.0f;
            }

            return (float)m_NumPerfectHits / ((float)m_NumPerfectHits + (float)m_NumMisses);
        }
    }
}
