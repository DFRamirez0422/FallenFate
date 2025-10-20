using UnityEngine;
<<<<<<< HEAD
using NPA_RhythmBonusPrefabs;

namespace NPA_PlayerPrefab.Scripts
{
    using RhythmTier = NPA_RhythmBonusPrefabs.RhythmBonusJudge.RhythmTier;
=======

namespace NPA_RhythmBonusPrefabs
{
    using RhythmTier =RhythmBonusJudge.RhythmTier;
>>>>>>> origin/JoseE_1015_PlayerCombat3

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

<<<<<<< HEAD
        [Tooltip("Reference to the rhythm evaluation system deemed the bonus judge.")]
        [SerializeField] private RhythmBonusJudge m_Judge;

        // Counts how many beats were hit in the current combo.
        private int m_ComboCounter = 0;

        // Counts how many total beats were hit perfectly.
        private int m_NumPerfectHits = 0;

        // Counts how many total beats were missed in rhythm.
        private int m_NumMisses = 0;
=======
        // Active flag. As long as this is set to true, the combos will count as normal.
        public bool m_IsActive = false;

        // ========================= Public Fields =========================//

        // Enables the beat counter mechanism, such as during the normal gameplay loop. By default, the
        // beat combo counter starts life inactive and must be enabled manually.
        public void Activate()
        {
            m_IsActive = true;
            Reset();
        }

        // Disable the beat counter mechanism, such as for example in a cutscene, player death, or other
        // event within the game where rhythm should not be counted.
        public void Deactivate()
        {
            m_IsActive = false;
            Reset();
        }
>>>>>>> origin/JoseE_1015_PlayerCombat3

        // Main function to be called to evaluate whether or not the music was hit on beat and updates
        // the counters accordingly. In other words, this is intended to be called by external code
        // to decide whether or not the player hit on beat and automatically counts up the combo
        // counter.
        //
        // Returns the tier - whether or not a perfect hit was done on beat or not.
        public RhythmTier EvaluateBeat()
        {
<<<<<<< HEAD
=======
            if (!m_IsActive) return RhythmTier.Miss;

>>>>>>> origin/JoseE_1015_PlayerCombat3
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

<<<<<<< HEAD
=======
            if (m_ComboCounter > m_MaxCombos)
            {
                m_MaxCombos = m_ComboCounter;
            }

            return tier;
        }

        // Query function that returns whether or not the caller is currently on beat to the music but without changing
        // any counters or combos. Can be used to quietly check the rhythm.
        public RhythmTier IsOnBeat()
        {
            if (!m_IsActive) return RhythmTier.Miss;

            var (tier, mult) = m_Judge.EvaluateNow(); // Call the judge
>>>>>>> origin/JoseE_1015_PlayerCombat3
            return tier;
        }

        // Reset all internal counters. Can be used if outside code wants a clean slate with combos and the number
        // of total hits and misses.
        public void Reset()
        {
            m_ComboCounter = 0;
<<<<<<< HEAD
=======
            m_MaxCombos = 0;
>>>>>>> origin/JoseE_1015_PlayerCombat3
            m_NumPerfectHits = 0;
            m_NumMisses = 0;
        }

        // Reset the combo counter by force. Can be used if outside code needs to reset a combo on purpose.
        public void ResetCombo()
        {
            m_ComboCounter = 0;
<<<<<<< HEAD
=======
            m_MaxCombos = 0;
>>>>>>> origin/JoseE_1015_PlayerCombat3
        }

        // Returns how many perfect hits were done on combo.
        public int GetCurrentCombo()
        {
            return m_ComboCounter;
        }

<<<<<<< HEAD
=======
        // Returns the highest combo recorded.
        public int GetMaxEverCombo()
        {
            return m_MaxCombos;
        }

>>>>>>> origin/JoseE_1015_PlayerCombat3
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
<<<<<<< HEAD
            return m_NumPerfectHits / (m_NumPerfectHits + m_NumMisses);
        }
=======
            // Special case to avoid a division by zero.
            if (m_NumPerfectHits == 0 || m_NumMisses == 0)
            {
                return 1.0f;
            }

            return (float)m_NumPerfectHits / ((float)m_NumPerfectHits + (float)m_NumMisses);
        }

        // ========================= Private Fields =========================//

        [Tooltip("Reference to the rhythm evaluation system deemed the bonus judge.")]
        [SerializeField] private RhythmBonusJudge m_Judge;
        
        [Header("Combo Bonuses")]

        [Tooltip("List of multiplier bonuses depending on the combo.")]
        [SerializeField] private float[] m_MultiplierBonuses = new float[] { 3, 6, 9 };

        [Tooltip("Specify how many hits to qualify for each bonus.")]
        [SerializeField] private int[] m_ComboCountBonuses = new int[] { 3, 6, 9 };

        // Counts how many beats were hit in the current combo.
        private int m_ComboCounter = 0;

        // Counts the highest combo ever recorded.
        private int m_MaxCombos = 0;

        // Counts how many total beats were hit perfectly.
        private int m_NumPerfectHits = 0;

        // Counts how many total beats were missed in rhythm.
        private int m_NumMisses = 0;
>>>>>>> origin/JoseE_1015_PlayerCombat3
    }
}
