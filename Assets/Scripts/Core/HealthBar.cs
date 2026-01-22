using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private TMP_Text m_HealthBarText;
    [SerializeField] private PlayerHealth m_Health;

    void Update()
    {
        m_HealthBarText.text = $"HP: {m_Health.CurrentHealth}/{m_Health.MaxHealth}";
    }
}
