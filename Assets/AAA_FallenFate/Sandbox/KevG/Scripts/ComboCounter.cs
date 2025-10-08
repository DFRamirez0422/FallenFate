using UnityEngine;
using TMPro;

public class ComboCounter : MonoBehaviour
{
    private int comboCount = 0;

    [Header("UI References")]
    public TextMeshProUGUI comboText;       // shows current combo count
    public TextMeshProUGUI finisherText;    // shows "Finisher Ready!"

    [Header("Finisher Settings")]
    public int finisherInterval = 3;   // every X combos, finisher is ready

    void Start()
    {
        UpdateComboUI();
        finisherText.text = "";
    }

    void Update()
    {
        // Detect mouse click or tap
        if (Input.GetMouseButtonDown(0))
        {
            AddHit();
        }
    }

    public void AddHit()
    {
        comboCount++;
        UpdateComboUI();

        // Check if comboCount is a multiple of finisherInterval
        if (comboCount % finisherInterval == 0)
        {
            finisherText.text = "Finisher Ready!";
        }
        else
        {
            finisherText.text = "";
        }
    }

    public void ResetCombo()
    {
        comboCount = 0;
        UpdateComboUI();
        finisherText.text = "";
    }

    private void UpdateComboUI()
    {
        if (comboText != null)
        {
            comboText.text = "" + comboCount;
        }
    }
}
