using UnityEngine;

public class FootstepPlayer : MonoBehaviour
{
    [Header("Footstep Data Asset")]
    [SerializeField] private SoundDefinition footstepSFX;
    
    // Called via Animation Event
    public void PlayFootstepSFX()
    {
        if (SoundFXManager.instance == null || footstepSFX == null) return;
        
        // Debug.Log("Footstep called");

        SoundFXManager.instance.Play(footstepSFX, transform);
    }
}
