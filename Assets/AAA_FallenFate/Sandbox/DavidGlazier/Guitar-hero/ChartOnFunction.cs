using System;
using System.IO;
using UnityEngine;
using ChartLoader.NET.Framework;
using ChartLoader.NET.Utils;
// If you're driving a UI Image/Text, uncomment this:
// using UnityEngine.UI;

public class ChartOnFunction : MonoBehaviour
{
    public enum HitTier { Miss, Good, Perfect }

    [Header("Chart Setup")]
    [Tooltip("Folder that contains the .chart file")]
    public string songFolder = "Assets/ChartLoader/ChartLoader/Songs/DOOM";

    [Tooltip("The .chart file name")]
    public string chartFileName = "notes.chart";

    [Tooltip("Section / difficulty name inside the chart (e.g., ExpertSingle, EasySingle, etc.)")]
    public string chartSection = "ExpertSingle";

    [Header("Look & Target")]
    [Tooltip("Renderer for 3D objects (leave null if using UI).")]
    public Renderer targetRenderer;

    // If you're using UI (Image/Text), drag the component here and leave targetRenderer null.
    // public UnityEngine.UI.Graphic uiGraphic;

    [Tooltip("Base 'off' material (used only for reading its color); not assigned to the object.")]
    public Material offRhymMat;

    [Tooltip("Peak 'on' material (used only for reading its color); not assigned to the object.")]
    public Material onRhymMat;

    [Tooltip("Use emission instead of/alongside base color when pulsing a 3D Renderer.")]
    public bool useEmission = true;

    [Header("Fade / Pulse")]
    [Tooltip("How quickly the pulse rises when a note hits (seconds).")]
    public float pulseAttack = 0.05f;

    [Tooltip("How quickly the pulse decays after a hit (seconds).")]
    public float pulseDecay = 0.35f;

    [Tooltip("Extra intensity on top of the base 'on' color at the pulse peak.")]
    public float pulseBoost = 1.0f;

    [Tooltip("Optional constant fade factor between off(0) and on(1) independent of pulses.")]
    [Range(0f, 1f)] public float baseFade = 0f;

    [Header("Judge Windows (seconds) & Offset")]
    [Tooltip("Max abs timing error for a Perfect.")]
    public float perfectWindow = 0.040f;   // 40 ms

    [Tooltip("Max abs timing error for a Good (above Perfect).")]
    public float goodWindow = 0.100f;      // 100 ms

    [Tooltip("Positive = judge later, Negative = judge earlier.")]
    public float latencyOffsetSec = 0f;

    private ChartReader chartReader;
    private Note[] notes = new Note[0];

    // Separate indices for visuals vs judging
    private int visualIdx = 0;
    private int judgeIdx = 0;

    private float songStartTime;
    private float lastHitTime = -999f;

    // cached colors from your materials so we don't modify shared assets
    private Color offColor = Color.gray;
    private Color onColor  = Color.white;

    // material property block (avoids instantiating materials on the renderer)
    private MaterialPropertyBlock mpb;

    void Awake()
    {
        mpb = new MaterialPropertyBlock();

        if (offRhymMat != null) offColor = GetColorFromMaterial(offRhymMat);
        if (onRhymMat  != null) onColor  = GetColorFromMaterial(onRhymMat);

        // Ensure emission keyword is on if we’ll drive it
        if (targetRenderer != null && useEmission)
        {
            foreach (var mat in targetRenderer.sharedMaterials)
            {
                if (mat != null) mat.EnableKeyword("_EMISSION");
            }
        }
    }

    void Start()
    {
        chartReader = new ChartReader();
        LoadChart(songFolder, chartFileName, chartSection);

        // Initialize look to "off"
        ApplyLook(0f);
    }

    void Update()
    {
        if (notes.Length == 0) return;

        float elapsed = Time.time - songStartTime;

        // VISUAL PULSE: trigger when we pass a note time (does not consume judge notes)
        while (visualIdx < notes.Length && notes[visualIdx].Seconds <= elapsed)
        {
            lastHitTime = Time.time;
            visualIdx++;
        }

        // Compute envelope amount (0..1): quick attack, exponential-ish decay
        float env = 0f;
        float dt = Time.time - lastHitTime;
        if (dt >= 0f)
        {
            if (dt <= pulseAttack)
            {
                // Linear rise to 1 over attack
                env = Mathf.Clamp01(dt / Mathf.Max(0.0001f, pulseAttack));
            }
            else
            {
                // Decay from 1 toward 0 after attack
                float decayT = (dt - pulseAttack) / Mathf.Max(0.0001f, pulseDecay);
                env = Mathf.Exp(-4f * decayT);
            }
        }

        // Combine with base fade (so you always have some blend toward 'on' if desired)
        float t = Mathf.Clamp01(baseFade + env);
        ApplyLook(t, env);

        // Example: quick mouse test (left click to evaluate a hit)
        if (Input.GetKeyDown(KeyCode.Mouse0)) // or: if (Input.GetMouseButtonDown(0))
        {
            var res = EvaluateHitNow();
            Debug.Log($"HIT: {res.tier}");
        }

    }

    private void LoadChart(string folder, string fileName, string section)
    {
        string fullPath = Path.Combine(folder, fileName);

        if (!File.Exists(fullPath))
        {
            Debug.LogError("Chart not found: " + fullPath);
            return;
        }

        Chart chart = chartReader.ReadChartFile(fullPath);
        Note[] loaded = chart?.GetNotes(section);

        if (loaded == null || loaded.Length == 0)
        {
            Debug.LogError("No notes found for section: " + section);
            return;
        }

        notes = loaded;
        Array.Sort(notes, (a, b) => a.Seconds.CompareTo(b.Seconds));
        visualIdx = 0;
        judgeIdx = 0;
        songStartTime = Time.time;

        Debug.Log($"Loaded chart: {fileName} ({section}), notes={notes.Length}");
    }

    /// =========================
    /// HIT EVALUATION API
    /// =========================

    /// <summary>
    /// Judge a player hit at the given song time (seconds).
    /// Returns (tier, signed error (sec), note index). Consumes note on Good/Perfect.
    /// </summary>
    public (HitTier tier, float errorSec, int noteIndex) EvaluateHit(double currentSongTimeSec)
    {
        if (notes == null || notes.Length == 0) return (HitTier.Miss, 0f, -1);
        if (judgeIdx >= notes.Length) return (HitTier.Miss, 0f, -1);

        // Apply calibration: shift the reference so we judge earlier/later
        double judgedTime = currentSongTimeSec + latencyOffsetSec;

        // 1) Skip notes that are far in the past beyond the Good window
        while (judgeIdx < notes.Length &&
               judgedTime - notes[judgeIdx].Seconds > goodWindow)
        {
            judgeIdx++;
        }
        if (judgeIdx >= notes.Length) return (HitTier.Miss, 0f, -1);

        // 2) Choose best candidate among the next few notes
        int bestIdx = judgeIdx;
        double bestAbsErr = Math.Abs(notes[bestIdx].Seconds - judgedTime);

        int lookAheadMax = Math.Min(judgeIdx + 2, notes.Length - 1);
        for (int i = judgeIdx + 1; i <= lookAheadMax; i++)
        {
            double err = Math.Abs(notes[i].Seconds - judgedTime);
            if (err < bestAbsErr)
            {
                bestAbsErr = err;
                bestIdx = i;
            }
        }

        float signedErr = (float)(judgedTime - notes[bestIdx].Seconds); // +late / -early

        // 3) Compare with windows, consume on success
        if (bestAbsErr <= perfectWindow)
        {
            judgeIdx = bestIdx + 1;
            // Optional: trigger a stronger visual pulse on successful hit
            lastHitTime = Time.time;
            return (HitTier.Perfect, signedErr, bestIdx);
        }
        if (bestAbsErr <= goodWindow)
        {
            judgeIdx = bestIdx + 1;
            lastHitTime = Time.time;
            return (HitTier.Good, signedErr, bestIdx);
        }

        // Not within Good: Miss (do not consume; player can still try the upcoming note if timing allows)
        return (HitTier.Miss, signedErr, -1);
    }

    /// <summary>
    /// Convenience: evaluate a hit using current Time.time relative to song start.
    /// Prefer feeding AudioSettings.dspTime if your audio is DSP-timed.
    /// </summary>
    public (HitTier tier, float errorSec, int noteIndex) EvaluateHitNow()
    {
        double t = Time.timeAsDouble - songStartTime;
        return EvaluateHit(t);
    }

    /// <summary>
    /// Applies a blended look between offColor and onColor.
    /// 't' is the overall fade 0..1. 'env' is the current pulse envelope (0..1) for extra boost.
    /// </summary>
    private void ApplyLook(float t, float env = 0f)
    {
        // Base color blend
        Color baseColor = Color.Lerp(offColor, onColor, t);

        // Add a pulse boost toward white (or toward onColor) at the peak
        Color boosted = Color.Lerp(baseColor, onColor * (1f + pulseBoost), env);

        if (targetRenderer != null)
        {
            targetRenderer.GetPropertyBlock(mpb);

            // Drive _Color
            mpb.SetColor("_Color", boosted);

            if (useEmission)
            {
                // Scale emission with envelope; feel free to tweak multiplier
                Color emission = onColor * (0.2f + env * (1.0f + pulseBoost));
                mpb.SetColor("_EmissionColor", emission);
            }

            targetRenderer.SetPropertyBlock(mpb);
        }
        // else if (uiGraphic != null)
        // {
        //     uiGraphic.color = boosted;
        // }
    }

    private static Color GetColorFromMaterial(Material mat)
    {
        if (mat.HasProperty("_Color"))
            return mat.GetColor("_Color");
        return Color.white;
    }
}
