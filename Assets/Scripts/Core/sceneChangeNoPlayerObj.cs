using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeNoPlayerObj : MonoBehaviour
{
    [Tooltip("Name of the scene to be loaded.")]
    [SerializeField] private string m_SceneName;

    [Tooltip("Animator component for the fade screen effect.")]
    [SerializeField] public Animator m_FadeScreenAnimator;

    [Tooltip("Amount of time to wait before loading the next scene.")]
    [SerializeField] private float m_FadeScreenTime = 0.5f;

    //public void TransitionSceneCutscene()
    //{
    //    if (m_FadeScreenAnimator)
    //    {
    //        m_FadeScreenAnimator.Play("FadeOut");
    //    }

    //    StartCoroutine(DelayFade());
    //}

    //private IEnumerator DelayFade()
    //{
    //    yield return new WaitForSeconds(m_FadeScreenTime);
    //    SceneManager.LoadScene(m_SceneName);
    //}
}
