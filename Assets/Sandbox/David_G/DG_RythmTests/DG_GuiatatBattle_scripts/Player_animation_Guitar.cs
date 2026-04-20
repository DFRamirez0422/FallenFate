using UnityEngine;
using Dypsloom.RhythmTimeline.Core.Notes;


public class Player_animation_Guitar : MonoBehaviour
{
    public Animator m_Animator;

    public void PlayAnim_PreStrum()
    {
        m_Animator.SetTrigger("preStrum");
    }

    public void PlayAnim_Strum()
    {
        m_Animator.SetTrigger("Strum");
    }

    public void PlayAnim_Miss()
    {
        m_Animator.SetTrigger("Hurt");
    }
}