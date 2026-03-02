using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public enum FloorType
    {
        Normal,
        Snow,
        Stairs,
        Bush,
    }

    [Tooltip("Game object audio source for oneshot sound effects playback.")]
    [SerializeField] private AudioSource m_SoundPlayer;

    [Header("Audio Clips")]
    [Tooltip("Normal footstep sounds for all terrain.")]
    [SerializeField] private AudioClip m_WalkNormalSound;
    [Tooltip("Foodstep audio clip for snow terrain.")]
    [SerializeField] private AudioClip m_WalkSnowSound;
    [Tooltip("Footstep audio clip for staircase terrain.")]
    [SerializeField] private AudioClip m_WalkStairSound;
    [Tooltip("Footstep audio clip for the bushes.")]
    [SerializeField] private AudioClip m_WalkBushSound;
    [Tooltip("Audio clip for slash attack.")]
    [SerializeField] private AudioClip m_AttackSound;
    [Tooltip("General sound for colliding with an object.")]
    [SerializeField] private AudioClip m_CollisionSound;
    [Tooltip("Audio clip for hitting the wall.")]
    [SerializeField] private AudioClip m_WallHitSound;
    [Tooltip("Audio clip for getting hit and attacked.")]
    [SerializeField] private AudioClip m_DamageSound;

    public void PlayFootstep()
    {
        if (!m_SoundPlayer) return;

        // TODO: somehow find a way to include the floor map code into the equation...
        FloorType sound_code = FloorType.Normal;
        AudioClip sound_clip;

        switch (sound_code)
        {
            case FloorType.Normal:
            default:
                sound_clip = m_WalkNormalSound;
                break; 

            case FloorType.Snow:
                sound_clip = m_WalkSnowSound;
                break;

            case FloorType.Stairs:
                sound_clip = m_WalkStairSound;
                break;

            case FloorType.Bush:
                sound_clip = m_WalkBushSound;
                break;
        }

        m_SoundPlayer.PlayOneShot(sound_clip);
    }

    public void PlayCollision()
    {
        if (!m_SoundPlayer) return;
        m_SoundPlayer.PlayOneShot(m_CollisionSound);
    }

    public void PlayWallHit()
    {
        if (!m_SoundPlayer) return;
        m_SoundPlayer.PlayOneShot(m_WallHitSound);
    }

    public void PlayDamage()
    {
        if (!m_SoundPlayer) return;
        m_SoundPlayer.PlayOneShot(m_DamageSound);
    }

    public void PlayAttack()
    {
        if (!m_SoundPlayer) return;
        m_SoundPlayer.PlayOneShot(m_AttackSound);
    }
}
