using UnityEngine;

public class DG_RythmCombat : MonoBehaviour
{
    [SerializeField] private AudioProcessor audioProcessor;
    
    [Header("Input Timing")]
    [Tooltip("Time window after beat where input is still valid (in seconds)")]
    [SerializeField] private float inputWindowAfterBeat = 0.2f;
    
    private float lastBeatTime = -999f;
    private bool inputUsedThisBeat = false;

    public int perfectHits = 0;

    [Tooltip("Specify how many hits to qualify for each bonus.")]
    [SerializeField] private int[] hitsTillFinisher = new int[] { 3, 6, 9 };

    void Start()
    {
        audioProcessor.onBeat.AddListener(OnBeatDetected);
    }

    void Update()
    {
        // Check for input within the timing window
        if (Input.GetKeyDown(KeyCode.H))
        {
            float timeSinceBeat = Time.time - lastBeatTime;
            
            // Check if we're within the valid window and haven't used input for this beat yet
            if (timeSinceBeat <= inputWindowAfterBeat && !inputUsedThisBeat)
            {
                OnSuccessfulRhythmInput();
            }
            else
            {
                OnMissedRhythmInput();
            }
        }
    }

    public void ResetPerfectHits()
        {
            perfectHits = 0;
        }

    private void OnBeatDetected()
    {   
        lastBeatTime = Time.time;
        inputUsedThisBeat = false;
    }
    
    private void OnSuccessfulRhythmInput()
    {
        inputUsedThisBeat = true;
        float timeSinceBeat = Time.time - lastBeatTime;
        Debug.Log($"HIT! Perfect Hits: {perfectHits} Timing: {timeSinceBeat:F3}s after beat");
        perfectHits++;
    }
    
    private void OnMissedRhythmInput()
    {
        float timeSinceBeat = Time.time - lastBeatTime;
        Debug.Log($"MISS! Perfect Hits: {perfectHits} Too late: {timeSinceBeat:F3}s after beat");
        perfectHits = 0;
    }
}
