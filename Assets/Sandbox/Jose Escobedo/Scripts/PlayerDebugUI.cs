using UnityEngine;

public class PlayerDebugUI : MonoBehaviour
{
    // UI text field that displays the current player state.
    [SerializeField] private TMPro.TextMeshProUGUI m_PlayerStateUI;

    // UI text field that displays the current player speed.
    [SerializeField] private TMPro.TextMeshProUGUI m_PlayerSpeedUI;

    // UI text field that displays the current unlocked special move.
    [SerializeField] private TMPro.TextMeshProUGUI m_AttackUnlockedUI;

    // UI text field that counts the current streak.
    [SerializeField] private TMPro.TextMeshProUGUI m_StreakUI;

    // UI text field that counts the current combo step.
    [SerializeField] private TMPro.TextMeshProUGUI m_ComboStepUI;

    // Sets the text to be displayed on screen about the current player state,
    public void SetDebugPlayerState(string value) => m_PlayerStateUI.text = value;

    // Sets the text to be displayed on screen about the current player speed,
    public void SetDebugPlayerSpeed(string value) => m_PlayerSpeedUI.text = value;

    // Sets the text to be displayed on screen about the current unlocked special move.
    public void SetDebugSpecialMoveUnlock(string value) => m_AttackUnlockedUI.text = value;

    // Sets the text to be displayed on screen about the current on-beat combo.
    public void SetDebugBeatStreak(string value) => m_StreakUI.text = value;

    public void SetDebugComboStep(string value) => m_ComboStepUI.text = value;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}