using System.Collections;
using UnityEngine;

public class PlayerFall : MonoBehaviour
{
    // Fall settings
    public float fallSpeed = 6f;
    public float fallTime = 1.0f;

    // Grace delay so the player has a short moment to escape
    public float fallGraceTime = 0.35f;
    
    ObjectFader fader; // reference to the ObjectFader component on the tile

    // State variables
    bool triggered;
    bool playerOnTile;

    void Start()
    {
        fader = GetComponent<ObjectFader>();
    }

    /// <summary>
    /// If the player is on the tile when it fades, they will fall through and lose 1 HP.
    /// TODO: The player will respawn at the last checkpoint after falling.
    /// </summary>
    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerOnTile = true;

        if (triggered) return;

        // Tile is faded
        if (fader.Mat.color.a <= 0.1f)
        {
            triggered = true;
            StartCoroutine(FallDelay(other.gameObject));
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerOnTile = false;
    }

    IEnumerator FallDelay(GameObject player)
    {
        float timer = 0f;

        while (timer < fallGraceTime)
        {
            if (!playerOnTile)
            {
                triggered = false;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(Fall(player));
    }

    /// <summary>
    /// Moves the player downwards for a short duration to simulate falling
    /// </summary>
    IEnumerator Fall(GameObject player)
    {
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();

        // disable player movement
        if (movement) movement.Disable();

        // move player behind island
        if (sr)
        { 
            sr.sortingOrder = -4;
            sr.sortingLayerName = "Back";
        }

        // move player downwards for a short duration to simulate falling
        float timer = 0f;
        while (timer < fallTime)
        {
            player.transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        // lose 1 HP
        if (health) health.ChangeHealth(-1);

        // TODO: respawn player and reset sorting order/layer
        if (movement) movement.RespawnPlayer();
        if (sr) sr.sortingOrder = 0;
        sr.sortingLayerName = "Character";

        triggered = false;
    }
}