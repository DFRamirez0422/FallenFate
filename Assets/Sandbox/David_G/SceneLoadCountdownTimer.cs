using UnityEngine;
using UnityEngine.Events;

public class SceneLoadCountdownTimer : MonoBehaviour
{
    [Header("Countdown Settings")]
    [SerializeField] private float m_StartTimeSeconds = 60;
    [Tooltip("If enabled, timer keeps counting even when Time.timeScale is 0 (paused). If disabled, timer uses normal game time and pauses with the game.")]
    [SerializeField] private bool m_UseUnscaledTime = false;

    [Header("Event Called At 00:00:00")]
    [SerializeField] private UnityEvent m_OnCountdownComplete;

    public float StartTimeSeconds
    {
        get => m_StartTimeSeconds;
        set => m_StartTimeSeconds = Mathf.Max(0.0f, value);
    }

    public string CurrentTimeFormatted => FormatTime(m_RemainingTimeSeconds);

    private float m_RemainingTimeSeconds;
    private bool m_IsRunning;
    private bool m_HasCompleted;

    private void Start()
    {
        StartCountdown();
    }

    private void Update()
    {
        if (!m_IsRunning || m_HasCompleted)
        {
            return;
        }

        float deltaTime = m_UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        m_RemainingTimeSeconds -= deltaTime;

        if (m_RemainingTimeSeconds <= 0f)
        {
            m_RemainingTimeSeconds = 0f;
            LogTime();
            CompleteCountdown();
            return;
        }

        LogTime();
    }

    public void StartCountdown()
    {
        m_RemainingTimeSeconds = Mathf.Max(0, m_StartTimeSeconds);
        m_IsRunning = true;
        m_HasCompleted = false;
        LogTime();

        if (m_RemainingTimeSeconds <= 0f)
        {
            CompleteCountdown();
        }
    }

    public void StopCountdown()
    {
        m_IsRunning = false;
    }

    private void CompleteCountdown()
    {
        if (m_HasCompleted)
        {
            return;
        }

        m_HasCompleted = true;
        m_IsRunning = false;
        m_OnCountdownComplete?.Invoke();
    }

    private void LogTime()
    {
        Debug.Log($"Countdown Timer: {CurrentTimeFormatted}");
    }

    private static string FormatTime(float totalSeconds)
    {
        int seconds = Mathf.Max(0, Mathf.CeilToInt(totalSeconds));
        int hours = seconds / 3600;
        int minutes = (seconds % 3600) / 60;
        int remainingSeconds = seconds % 60;
        return $"{hours:00}:{minutes:00}:{remainingSeconds:00}";
    }
}
