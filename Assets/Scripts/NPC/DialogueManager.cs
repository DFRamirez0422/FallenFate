using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Schema;

public class DialogueManager : MonoBehaviour
{
    // ===== USER INTERFACE FIELDS ===== //
    [Header("UI References")]
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Image m_Portrait;
    [SerializeField] private TMP_Text m_ActorName;
    [SerializeField] private TMP_Text m_DialogueText;
    [SerializeField] private Button[] m_ChoiceButtons;


    // ===== PUBLIC FIELDS ===== //
    public static DialogueManager Instance;
    public bool IsDialogueActive = false;


    // ===== PRIVATE FIELDS ===== //
    private DialogueSO m_CurrentDialogue;
    private int m_DialogueIdx;


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

        m_CanvasGroup = GetComponent<CanvasGroup>();
        m_CanvasGroup.alpha = 0;
        m_CanvasGroup.interactable = false;
        m_CanvasGroup.blocksRaycasts = false;

        foreach (var button in m_ChoiceButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void StartDialogue(DialogueSO dialogueSO)
    {
        m_CurrentDialogue = dialogueSO;
        m_DialogueIdx = 0;
        IsDialogueActive = true;
        ShowDialogue();
    }

    public void AdvanceDialogue()
    {
        if (m_DialogueIdx < m_CurrentDialogue.lines.Length)
        {
            ShowDialogue();
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

        m_Portrait.sprite = line.speaker.m_Portrait;
        m_ActorName.text = line.speaker.m_ActorName;
        m_DialogueText.text = line.text;
        m_DialogueIdx++;

        m_CanvasGroup.alpha = 1;
        m_CanvasGroup.interactable = true;
        m_CanvasGroup.blocksRaycasts = true;
    }

    private void EndDialogue()
    {
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
            m_ChoiceButtons[0].GetComponentInChildren<TMP_Text>().text = "End";
            m_ChoiceButtons[0].onClick.AddListener(EndDialogue);
            m_ChoiceButtons[0].gameObject.SetActive(true);
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
}
