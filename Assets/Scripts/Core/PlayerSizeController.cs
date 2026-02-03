using UnityEngine;

/// <summary>
/// Finds the player in the scene and sets their scale to a specified size.
/// </summary>
public class PlayerSizeController : MonoBehaviour
{
    [Tooltip("The scale/size to set the player to. (1,1,1) is normal size.")]
    [SerializeField] private Vector3 m_PlayerSize = Vector3.one;
    
    [Tooltip("Whether to apply the size on Start.")]
    [SerializeField] private bool m_ApplyOnStart = true;

    private Transform m_Player;

    void Start()
    {
        FindPlayer();
        
        if (m_ApplyOnStart && m_Player != null)
        {
            ApplySize();
        }
    }

    /// <summary>
    /// Finds the player GameObject by tag.
    /// </summary>
    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObject != null)
        {
            m_Player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player GameObject has the 'Player' tag.");
        }
    }

    /// <summary>
    /// Applies the specified size to the player.
    /// </summary>
    public void ApplySize()
    {
        if (m_Player != null)
        {
            m_Player.localScale = m_PlayerSize;
            Debug.Log($"Player size set to: {m_PlayerSize}");
        }
        else
        {
            Debug.LogWarning("Cannot apply size - Player not found!");
        }
    }

    /// <summary>
    /// Sets a new size and applies it immediately.
    /// </summary>
    /// <param name="newSize">The new size to apply.</param>
    public void SetPlayerSize(Vector3 newSize)
    {
        m_PlayerSize = newSize;
        ApplySize();
    }
}
