using UnityEngine;
using NPA_RhythmBonusPrefabs;
using Tier = NPA_RhythmBonusPrefabs.RhythmBonusJudge.RhythmTier;

/// <summary>
/// Quick debug helper: press a key to test
/// </summary>
public class BeatCounterUI : MonoBehaviour
{
    [SerializeField] private KeyCode m_BeatKey = KeyCode.Mouse0; // Key to sim "attack"

    [SerializeField] private KeyCode m_ResetKey = KeyCode.Mouse1; // Key to reset manually during testing

    [SerializeField] private KeyCode m_StaffAppearKey = KeyCode.Mouse2; // Key to test music bar UI activation

    [SerializeField] private RhythmMusicPlayer m_MusicPlayer;

    [SerializeField] private BeatComboCounter m_ComboCounter;

    [SerializeField] private MusicBarUI m_MusicStaff;

    // UI text field that displays the current player state.
    [SerializeField] private TMPro.TextMeshProUGUI m_PlayerStateUI;

    // UI text field that counts the number of perfect hits.
    [SerializeField] private TMPro.TextMeshProUGUI m_NumHitsPerfectUI;

    // UI text field that counts the number of good hits.
    [SerializeField] private TMPro.TextMeshProUGUI m_GradeUI;

    // UI text field that counts the number of misses.
    [SerializeField] private TMPro.TextMeshProUGUI m_NumMissesUI;

    // UI text field that counts the current streak.
    [SerializeField] private TMPro.TextMeshProUGUI m_StreakUI;

    // UI text field that counts the highest combo.
    [SerializeField] private TMPro.TextMeshProUGUI m_MaxComboUI;

    private bool m_IsActive = false;

    /*
        End variable section from Jose E.
    */

    void Update()
    {
        // When key is pressed, evaluate rhythm and print result
        if (Input.GetKeyDown(m_BeatKey))
        {
            var tier = m_ComboCounter.EvaluateBeat();
            string tier_name = System.Enum.GetName(typeof(Tier), tier); // Retrieve the name of the tier for printing.

            // Below section added by Jose E.
            // This showcases how we might integrate the evaluation results to the rest of the
            // game. The question is, how does external code refer to the results?
            switch (tier)
            {
                case Tier.Perfect:
                    tier_name = "Perfect";
                    break;
                case Tier.Good:
                    tier_name = "Good";
                    break;
                case Tier.Miss:
                default:
                    tier_name = "Miss";
                    break;
            }

            Debug.Log($"[Rhyhtm Test] {tier_name} hit");
        }
        if (Input.GetKeyDown(m_ResetKey))
        {
            m_ComboCounter.Reset();
        }
        if (Input.GetKeyDown(m_StaffAppearKey))
        {
            if (m_IsActive)
            {
                m_ComboCounter.Deactivate();
                m_MusicStaff.Deactivate();
                m_IsActive = false;
            }
            else
            {
                m_ComboCounter.Activate();
                m_MusicStaff.gameObject.SetActive(true);
                m_MusicStaff.Activate();
                m_IsActive = true;
            }
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
        if (!m_PlayerStateUI) return;

        m_NumHitsPerfectUI.text = m_ComboCounter.GetNumberOfPerfects().ToString();
        m_NumMissesUI.text = m_ComboCounter.GetNumberOfMisses().ToString();
        m_StreakUI.text = m_ComboCounter.GetCurrentCombo().ToString();
        m_GradeUI.text = $"{m_ComboCounter.GetTimingGrade() * 100.0f:0.}%";
        m_MaxComboUI.text = m_ComboCounter.GetMaxEverCombo().ToString();
    }

    // ===== ONLY FOR TESTING =====
    // 
    // Sets the text to be displayed on screen about the current player state,
    public void SetDebugPlayerState(string value)
    {
        m_PlayerStateUI.text = value;
    }
}