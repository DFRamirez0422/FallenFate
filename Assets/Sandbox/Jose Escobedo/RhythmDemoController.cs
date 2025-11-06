using UnityEngine;
using NPA_RhythmBonusPrefabs;

public class RhythmDemoController : MonoBehaviour
{
    /*
        A very basic controller meant to be used only during testing and development to quickly have
        a working UI and rhythm system before anything else is finished.

        Obviously goes without saying that this is ONLY FOR TESTING.
    */
    [SerializeField] private MusicBarUI m_MusicStaff;
    [SerializeField] private BeatComboCounter m_ComboCounter;

    [SerializeField] private KeyCode m_ResetKey = KeyCode.Mouse1; // Key to reset manually during testing

    [SerializeField] private KeyCode m_StaffAppearKey = KeyCode.Mouse2; // Key to test music bar UI activation

    private bool m_IsActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {
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
    }
}
