using UnityEngine;
using Dypsloom.RhythmTimeline.Core.Notes;


public class Player_animation_Guitar : MonoBehaviour
{
    public Animator m_Animator;

    public void PlayAnim_PreStrum()
    {
        m_Animator.Play("Pre_Strum_Player_Guitar");
    }

    public void PlayAnim_Strum()
    {
        m_Animator.Play("Strum_Player_guitar");
    }

    public void PlayAnim_Miss()
    {
        m_Animator.Play("TakeDamage_Player_guitar");
    }
}