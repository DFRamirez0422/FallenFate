using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComboCounter : MonoBehaviour
{
    private int comboCount = 0;
    private int finisherStage = 0;

    [Header("UI References")]
    public TextMeshProUGUI comboText;
    public Image finisherBar;
    public TextMeshProUGUI finisherStageText;

    [Header("Finisher Settings")]
    public int hitsPerStage = 3;    // Every X hits fills one stage
    public int maxStages = 3;       // Total stages (e.g., 3 = 9 hits for full bar)

    private int maxCombo; // Will be calculated in Start()

    void Start()
    {
        maxCombo = hitsPerStage * maxStages;  // e.g., 3 × 3 = 9
        UpdateComboUI();
        UpdateFinisherBar();
        UpdateFinisherStageText();
    }

    void Update()
    {
        // Click or tap to add combo hits
        if (Input.GetMouseButtonDown(0))
        {
            AddHit();
        }

        // Space key to use finisher
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UseFinisher();
        }
    }

    public void AddHit()
    {
        // Prevent going over max combo
        if (comboCount >= maxCombo)
            return;

        comboCount++;
        UpdateComboUI();
        UpdateFinisherBar();

        // Increase stage at thresholds
        if (comboCount % hitsPerStage == 0 && finisherStage < maxStages)
        {
            finisherStage++;
            UpdateFinisherStageText();
        }
    }

    public void UseFinisher()
    {
        if (finisherStage > 0)
        {
            Debug.Log("Finisher used at stage " + finisherStage);

            // Reset for refill
            comboCount = 0;
            finisherStage = 0;
            UpdateComboUI();
            UpdateFinisherBar();
            UpdateFinisherStageText();
        }
    }

    private void UpdateComboUI()
    {
        if (comboText != null)
            comboText.text = "" + comboCount ;
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

    private void UpdateFinisherStageText()
    {
        if (finisherStageText != null)
            finisherStageText.text = "Stage " + finisherStage + " / " + maxStages;
    }
}
