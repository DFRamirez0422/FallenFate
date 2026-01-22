using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [Tooltip("Name of the scene to be loaded upon trigger.")]
    [SerializeField] private string m_SceneName;
    [Tooltip("Position in the new scene to position the player.")]
    [SerializeField] private Vector2 m_DestPosition;
    [Tooltip("Animator component for the fader screen effect.")]
    [SerializeField] private Animator m_FadeScreenAnimator;
    [Tooltip("Amount of time to wait between scene transitions.")]
    [SerializeField] private float m_FadeScreenTime = 0.5f;

    private Transform m_Player;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_Player = collision.transform;
            m_FadeScreenAnimator.Play("FadeOut");
            StartCoroutine(DelayFade());
        }
    }

    IEnumerator DelayFade()
    {
        yield return new WaitForSeconds(m_FadeScreenTime);
        m_Player.position = m_DestPosition;
        SceneManager.LoadScene(m_SceneName);
    }
}
