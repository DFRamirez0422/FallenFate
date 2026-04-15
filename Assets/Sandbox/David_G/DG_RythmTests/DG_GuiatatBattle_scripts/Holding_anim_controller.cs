using UnityEngine;
using Dypsloom.RhythmTimeline.Core.Notes;

public class Holding_anim_controller : MonoBehaviour
{
    [SerializeField] private Animator m_Animator;

    public HoldNote m_HoldNote;

    private void Start()
    {
        m_Animator = GameObject.FindWithTag("Player").GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (m_HoldNote.IsHolding == true)
        {
            m_Animator.Play("Strum_Player_guitar");
        }
    }
}
