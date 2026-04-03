using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    // ===== USER INTERFACE FIELDS ===== //
    [Header("UI References")]
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Image m_Portrait;
    [SerializeField] private TMP_Text m_ActorName;
    [SerializeField] private TMP_Text m_DialogueText;
    [SerializeField] private Button[] m_ChoiceButtons;
    [SerializeField] private Button m_ContinueButton;
    [SerializeField] private Button m_EndButton;
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
    private bool m_IsRevealingText;


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
        m_IsRevealingText = false;

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

        if (m_IsRevealingText)
        {
            if (update_tick <= line_length)
            {
                m_DialogueText.text = line.text.Substring(0, update_tick);
                m_LineUpdateTick += delta_time;
                m_IsRevealingText = true;
            }
            else
            {
                m_IsRevealingText = false;
            }
        }

        // Centralize dialogue progression input so only one script advances each key press.
        if (IsDialogueActive && Input.GetButtonDown("Interact"))
        {
            if (m_IsRevealingText)
            {
                m_DialogueText.text = line.text;
                m_IsRevealingText = false;
            }
            else
            {
                AdvanceDialogue();
            }
        }
        else if (IsDialogueActive && Input.GetButtonDown("Attack"))
        {
            EndDialogue();
        }
    }

    public void StartDialogue(DialogueSO dialogueSO)
    {
        if (dialogueSO == null || dialogueSO.lines == null || dialogueSO.lines.Length == 0)
        {
            return;
        }

        // Disable all player movement when the dialogue screen is open.
        m_Player.GetComponent<PlayerMovement>().Disable();

        // Fully pause gameplay while dialogue is open.
        Time.timeScale = 0.0f;

        m_CurrentDialogue = dialogueSO;
        m_DialogueIdx = 0;
        IsDialogueActive = true;
        ShowDialogue();
        UpdateActionText();
    }

    public void AdvanceDialogue()
    {
        if (!IsDialogueActive || m_CurrentDialogue == null || m_CurrentDialogue.lines == null)
        {
            return;
        }

        if (m_DialogueIdx + 1 < m_CurrentDialogue.lines.Length)
        {
            m_DialogueIdx++;
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
        if (m_CurrentDialogue == null || m_CurrentDialogue.lines == null || m_DialogueIdx >= m_CurrentDialogue.lines.Length)
        {
            return;
        }

        DialogueLine line = m_CurrentDialogue.lines[m_DialogueIdx];
        DialogueHistoryTracker.Instance.RecordNPC(line.speaker);
        m_LineUpdateTick = 0;
        m_IsRevealingText = true;

        SetPortraitByEmotion(line);
        m_ActorName.text = line.speaker.m_ActorName;
        m_DialogueText.text = line.text;

        m_CanvasGroup.alpha = 1;
        m_CanvasGroup.interactable = true;
        m_CanvasGroup.blocksRaycasts = true;
    }

    private void EndDialogue()
    {
        // Start a behavior to be triggered upon dialogue ending.
        OnDialogueEnd();

        m_DialogueIdx = 0;
        m_IsRevealingText = false;
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
            m_ContinueButton.gameObject.SetActive(false);
            m_ContinueButton.onClick.RemoveAllListeners();

            m_EndButton.gameObject.SetActive(false);
            m_EndButton.onClick.RemoveAllListeners();

            int choiceCount = Mathf.Min(m_CurrentDialogue.options.Length, m_ChoiceButtons.Length);
            for (int i = 0; i < choiceCount; i++)
            {
                var option = m_CurrentDialogue.options[i];
                m_ChoiceButtons[i].GetComponentInChildren<TMP_Text>().text = option.optionText;
                m_ChoiceButtons[i].gameObject.SetActive(true);
                m_ChoiceButtons[i].onClick.AddListener(MakeChoiceHandler(option));
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

    private UnityEngine.Events.UnityAction MakeChoiceHandler(DialogueOption option)
    {
        switch(option.action)
        {
            case DialogueOption.Action.NewDialogue:
                return () => ChooseOption(option.nextDialogue);

            case DialogueOption.Action.ChangeScene:
                return () =>
                {
                    EndDialogue();
                    SceneManager.LoadScene(option.sceneName);
                };

            default:
                Debug.LogError("ERROR : Unknown option type for dialogue option.");
                return () => EndDialogue();
        }
    }

    private void ClearChoices()
    {
        foreach (var button in m_ChoiceButtons)
        {
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }
        
        m_ContinueButton.onClick.RemoveAllListeners();
        m_EndButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Updates the action button text to prompt the user to continue or end the dialogue with a key press.
    /// </summary>
    private void UpdateActionText()
    {
        if (m_CurrentDialogue == null)
        {
            return;
        }

        m_EndButton.gameObject.SetActive(true);
        m_EndButton.onClick.RemoveAllListeners();
        m_EndButton.onClick.AddListener(EndDialogue);

        // Check if at the end of the dialogue tree (no more lines, no choices).
        bool atEnd = m_DialogueIdx + 1 >= m_CurrentDialogue.lines.Length && m_CurrentDialogue.options.Length == 0;
        if (atEnd)
        {
            m_ContinueButton.gameObject.SetActive(false);
        }
        else
        {
            m_ContinueButton.gameObject.SetActive(true);
            m_ContinueButton.onClick.RemoveAllListeners();
            m_ContinueButton.onClick.AddListener(AdvanceDialogue);
        }
    }

    /// <summary>
    /// Sets the proper portraits based on the current dialogue line's emotion.
    /// </summary>
    /// <param name="line"></param>
    private void SetPortraitByEmotion(DialogueLine line)
    {
        foreach (ActorSO.EmotionPortrait emotion_portrait in line.speaker.m_EmotionPortraits)
        {
            if (emotion_portrait.emotion == line.emotion)
            {
                m_Portrait.sprite = emotion_portrait.portrait;
                return;
            }
        }

        m_Portrait.sprite = line.speaker.m_DefaultPortrait;
    }

    /// <summary>
    /// `OnDialougeEnd` only runs when the player has completed the dialouge sequnce with the NPC they we're just talking to.
    /// </summary>
    private void OnDialogueEnd()
    {
        // Enable all player movement when the dialogue is finished.
        m_Player.GetComponent<PlayerMovement>().Enable();
        // Restore AI movement (revert from near-pause used during dialogue).
        Time.timeScale = 1.0f;

        switch(m_CurrentDialogue.actionOnDialogueEnd)
        {
            case DialogueSO.ActionOnEnd.EndDialogue:
                break;

            case DialogueSO.ActionOnEnd.NewDialogue:
                StartDialogue(nextDialogue);
                break;

            case DialogueSO.ActionOnEnd.ChangeScene:
                SceneManager.LoadScene(m_CurrentDialogue.sceneName);
                break;

            case DialogueSO.ActionOnEnd.SetObjectsActive:
                foreach (GameObject obj in m_CurrentDialogue.m_ObjectsToSetActive)
                {
                    obj.SetActive(true);
                }
                break;

            case DialogueSO.ActionOnEnd.InstantiateObjects:
                foreach (GameObject obj in m_CurrentDialogue.m_ObjectsToInstantiate)
                {
                    Instantiate(obj);
                }
                break;

            default:
                Debug.Log("no ending behavior set in current dialouge");
                break;
        }
    }
}
