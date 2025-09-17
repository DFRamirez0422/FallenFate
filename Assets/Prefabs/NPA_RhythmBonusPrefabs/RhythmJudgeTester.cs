using UnityEngine;

namespace Player.RhythmBonusPrefabs
{
    using Tier = RhythmBonusJudge.RhythmTier;

    /// <summary>
    /// Quick debug helper: press a key to test
    /// </summary>
    public class RyhthmJudgeTester : MonoBehaviour
    {
        [SerializeField] private RhythmBonusJudge judge;
        [SerializeField] private KeyCode testKey = KeyCode.Mouse0; // Key to sim "attack"
        
        /*
            Below variables added by Jose E. for testing.
        */

        [SerializeField] private RhythmMusicPlayer m_MusicPlayer;

        // UI text field that displays the song tempo in BPM.
        [SerializeField] private TMPro.TextMeshProUGUI m_TempoUI;

        // UI text field that counts the number of perfect hits.
        [SerializeField] private TMPro.TextMeshProUGUI m_NumHitsPerfectUI;

        // UI text field that counts the number of good hits.
        [SerializeField] private TMPro.TextMeshProUGUI m_NumHitsGoodUI;

        // UI text field that counts the number of misses.
        [SerializeField] private TMPro.TextMeshProUGUI m_NumMissesUI;

        // UI text field that counts the current streak.
        [SerializeField] private TMPro.TextMeshProUGUI m_StreakUI;

        // Counts how many beats were hit in the current streak.
        private int m_HitStreak = 0;

        // Counts how many total beats were hit perfectly.
        private int m_NumPerfectHits = 0;

        // Counts how many total beats were hit decently.
        private int m_NumGoodHits = 0;

        // Counts how many total beats were missed in rhythm.
        private int m_NumMisses = 0;

        /*
            End variable section from Jose E.
        */
        
        void Update()
        {
            // When key is pressed, evaluate rhythm and print result
            if (Input.GetKeyDown(testKey))
            {
                var (tier, mult) = judge.EvaluateNow(); // Call the judge
                string tier_name = System.Enum.GetName(typeof(Tier), tier); // Retrieve the name of the tier for printing.

                // Below section added by Jose E.
                // This showcases how we might integrate the evaluation results to the rest of the
                // game. The question is, how does external code refer to the results?
                switch (tier)
                {
                    case Tier.Perfect:
                        tier_name = "Perfect";
                        m_NumPerfectHits += 1;
                        m_HitStreak += 1;
                        break;
                    case Tier.Good:
                        tier_name = "Good";
                        m_NumGoodHits += 1;
                        m_HitStreak += 1;
                        break;
                    case Tier.Miss:
                    default:
                        tier_name = "Miss";
                        m_NumMisses += 1;
                        m_HitStreak = 0;
                        break;
                }

                Debug.Log($"[Rhyhtm Test] {tier_name} hit, Multiplier = x{mult}");
            }

            // Below section added by Jose E.
            UpdateUI();
        }
        
        // Added by Jose E.
        // Updates the UI to display information to the player concerning rhythm.
        //
        // TODO: ONLY USED FOR TESTING : please remove when finished.
        private void UpdateUI()
        {
            // If the UI doesn't exist, return immediately.
            if (!m_TempoUI) return;

            m_TempoUI.text = m_MusicPlayer.BPM.ToString();
            m_NumHitsPerfectUI.text = m_NumPerfectHits.ToString();
            m_NumHitsGoodUI.text = m_NumGoodHits.ToString();
            m_NumMissesUI.text = m_NumMisses.ToString();
            m_StreakUI.text = m_HitStreak.ToString();
        }
    }
}