using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComboCounter : MonoBehaviour
{
    private int comboCount = 0;
    private int finisherStage = 0;
    private int maxCombo;

    [Header("UI References")]
    public TextMeshProUGUI comboText;
    public Image finisherBar;

    [Header("Stage Texts")]
    public TextMeshProUGUI stage1Text; // Light Finisher
    public TextMeshProUGUI stage2Text; // Heavy Finisher
    public TextMeshProUGUI stage3Text; // Ultimate Finisher

    [Header("Finisher Settings")]
    public int hitsPerStage = 3;    // hits needed per stage
    public int maxStages = 3;       // total stages (3 = 9 hits for full bar)

    void Start()
    {
        maxCombo = hitsPerStage * maxStages;

        ResetStageTexts();
        UpdateComboUI();
        UpdateFinisherBar();
    }

    void Update()
    {
        // Click or tap to build combo
        if (Input.GetMouseButtonDown(0))
        {
            AddHit();
        }

        // Spacebar to use finisher
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UseFinisher();
        }
    }

    public void AddHit()
    {
        if (comboCount >= maxCombo)
            return; // stop counting at max combo

        comboCount++;
        UpdateComboUI();
        UpdateFinisherBar();

        // Check if we reached a new stage
        if (comboCount % hitsPerStage == 0 && finisherStage < maxStages)
        {
            finisherStage++;
            UpdateStageTexts();
        }
    }

    public void UseFinisher()
    {
        if (finisherStage > 0)
        {
            Debug.Log("Finisher used at stage " + finisherStage);

            // Reset everything for refill
            comboCount = 0;
            finisherStage = 0;

            ResetStageTexts();
            UpdateComboUI();
            UpdateFinisherBar();
        }
    }

    private void UpdateComboUI()
    {
        if (comboText != null)
        {
            comboText.text = "" + comboCount;
        }
    }

    private void UpdateFinisherBar()
    {
        if (finisherBar != null)
        {
            float progress = (float)comboCount / maxCombo;
            progress = Mathf.Clamp01(progress);
            finisherBar.fillAmount = progress;
        }
    }

    private void ResetStageTexts()
    {
        if (stage1Text != null) stage1Text.gameObject.SetActive(false);
        if (stage2Text != null) stage2Text.gameObject.SetActive(false);
        if (stage3Text != null) stage3Text.gameObject.SetActive(false);
    }

    private void UpdateStageTexts()
    {
        // Turn them all off first
        ResetStageTexts();

        // Enable the one that matches the current stage
        if (finisherStage == 1 && stage1Text != null) stage1Text.gameObject.SetActive(true);
        if (finisherStage == 2 && stage2Text != null) stage2Text.gameObject.SetActive(true);
        if (finisherStage == 3 && stage3Text != null) stage3Text.gameObject.SetActive(true);
    }
}