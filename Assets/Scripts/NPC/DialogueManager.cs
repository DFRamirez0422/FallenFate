using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    // ===== USER INTERFACE FIELDS ===== //
    [Header("UI References")]
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Image m_Portrait;
    [SerializeField] private TMP_Text m_ActorName;
    [SerializeField] private TMP_Text m_DialogueText;
    [SerializeField] private Button[] m_ChoiceButtons;
    [SerializeField] private Button m_ActionButton;
    [Header("Dialogue Control")]
    [Tooltip("Amount of time in between each letter reveal, measured in milliseconds.")]
    [SerializeField] private int m_TextRevealSpeed = 30;


    // ===== PUBLIC FIELDS ===== //
    public static DialogueManager Instance;
    public bool IsDialogueActive = false;


    // ===== PRIVATE FIELDS ===== //
    private GameObject m_Player;
    private DialogueSO m_CurrentDialogue;
    private int m_DialogueIdx;
    private float m_LineUpdateTick; // Dialogue should reveal slowly, not all at once. This counter helps keep track what to show.
    private float m_LastLineUpdateTime;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_CanvasGroup = GetComponent<CanvasGroup>();
        m_CanvasGroup.alpha = 0;
        m_CanvasGroup.interactable = false;
        m_CanvasGroup.blocksRaycasts = false;

        foreach (var button in m_ChoiceButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        float delta_time = Time.realtimeSinceStartup - m_LastLineUpdateTime;
        m_LastLineUpdateTime = Time.realtimeSinceStartup;

        if (!m_CurrentDialogue) return;
        
        // Algorithm overview:
        // Each amount of update ticks, reveal one letter at a time until the whole line is displayed.
        DialogueLine line = m_CurrentDialogue.lines[m_DialogueIdx];
        int line_length = line.text.Length;
        int update_tick = (int)(1000.0f * m_LineUpdateTick / m_TextRevealSpeed);

        if (update_tick <= line_length)
        {
            m_DialogueText.text = line.text.Substring(0, update_tick);
            m_LineUpdateTick += delta_time;
        }
    }

    public void StartDialogue(DialogueSO dialogueSO)
    {
        // Disable all player movement when the dialogue screen is open.
        m_Player.GetComponent<PlayerMovement>().Disable();

        // Disable all AI movement.
        Time.timeScale = 0.0f;

        m_CurrentDialogue = dialogueSO;
        m_DialogueIdx = 0;
        IsDialogueActive = true;
        ShowDialogue();
        UpdateActionText();
    }

    public void AdvanceDialogue()
    {
        m_DialogueIdx++;

        if (m_DialogueIdx < m_CurrentDialogue.lines.Length)
        {
            ShowDialogue();
            UpdateActionText();
        }
        else
        {
            ShowChoices();
        }
    }

    private void ShowDialogue()
    {
        DialogueLine line = m_CurrentDialogue.lines[m_DialogueIdx];
        DialogueHistoryTracker.Instance.RecordNPC(line.speaker);
        m_LineUpdateTick = 0;

        m_Portrait.sprite = line.speaker.m_Portrait;
        m_ActorName.text = line.speaker.m_ActorName;
        m_DialogueText.text = line.text;

        m_CanvasGroup.alpha = 1;
        m_CanvasGroup.interactable = true;
        m_CanvasGroup.blocksRaycasts = true;
    }

    private void EndDialogue()
    {
        // Enable all player movement when the dialogue is finished.
        m_Player.GetComponent<PlayerMovement>().Enable();

        // Enable all AI movement.
        Time.timeScale = 1.0f;

        m_DialogueIdx = 0;
        IsDialogueActive = false;
        ClearChoices();

        m_CanvasGroup.alpha = 0;
        m_CanvasGroup.interactable = false;
        m_CanvasGroup.blocksRaycasts = false;
    }

    private void ShowChoices()
    {
        ClearChoices();

        if (m_CurrentDialogue.options.Length > 0)
        {
            m_ActionButton.gameObject.SetActive(false);
            m_ActionButton.onClick.RemoveAllListeners();

            for (int i = 0; i < m_CurrentDialogue.options.Length; i++)
            {
                var option = m_CurrentDialogue.options[i];
                m_ChoiceButtons[i].GetComponentInChildren<TMP_Text>().text = option.optionText;
                m_ChoiceButtons[i].gameObject.SetActive(true);
                m_ChoiceButtons[i].onClick.AddListener(() => ChooseOption(option.nextDialogue));
            }
        }
        else
        {
            EndDialogue();
        }
    }

    private void ChooseOption(DialogueSO dialogueSO)
    {
        if (dialogueSO == null)
        {
            EndDialogue();
        }
        else
        {
            ClearChoices();
            StartDialogue(dialogueSO);
        }
    }

    private void ClearChoices()
    {
        foreach (var button in m_ChoiceButtons)
        {
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }
    }

    /// <summary>
    /// Updates the action button text to prompt the user to continue or end the dialogue with a key press.
    /// </summary>
    private void UpdateActionText()
    {
        m_ActionButton.onClick.RemoveAllListeners();
        m_ActionButton.onClick.AddListener(EndDialogue);
        m_ActionButton.gameObject.SetActive(true);

        // Check if at the end of the dialogue tree.
        if (m_DialogueIdx >= m_CurrentDialogue.lines.Length && m_CurrentDialogue.options.Length == 0)
        {
            m_ActionButton.GetComponentInChildren<TMP_Text>().text = "[x] End Dialogue";
        }
        else
        {
            m_ActionButton.GetComponentInChildren<TMP_Text>().text = "[x] Continue";
        }
    }
}
