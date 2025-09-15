using UnityEngine;

namespace Player.RhythmBonusPrefabs
{
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
                Debug.Log($"[Rhyhtm Test] {tier} hit, Multiplier = x{mult}");

                // Below section added by Jose E.
                switch (tier)
                {
                    case "Perfect":
                        m_NumPerfectHits += 1;
                        m_HitStreak += 1;
                        break;
                    case "Good":
                        m_NumGoodHits += 1;
                        m_HitStreak += 1;
                        break;
                    case "Miss":
                    default:
                        m_NumMisses += 1;
                        m_HitStreak = 0;
                        break;
                }
            }

            // Below section added by Jose E.
            UpdateUI();
        }
        
        // Added by Jose E.
        // Updates the UI to display information to the player concerning rhythm.
        private void UpdateUI()
        {
            m_TempoUI.text = m_MusicPlayer.BPM.ToString();
            m_NumHitsPerfectUI.text = m_NumPerfectHits.ToString();
            m_NumHitsGoodUI.text = m_NumGoodHits.ToString();
            m_NumMissesUI.text = m_NumMisses.ToString();
            m_StreakUI.text = m_HitStreak.ToString();
        }
    }
}