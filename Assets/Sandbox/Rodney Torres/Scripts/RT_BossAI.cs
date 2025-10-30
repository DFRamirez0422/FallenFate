using UnityEngine;
using System.Collections;

public class RT_BossAI : MonoBehaviour
{
    [Header("Arena Setup")]
    public float bossCutsceneTimer = 8f;
    public BoxCollider arenaTrigger; // (unused if you use a trigger proxy)

    private bool playerEnteredArena = false;

    private SpriteRenderer bossSprite;
    private Color originalColor;

    [Header("Music Setup")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bossSong;
    [SerializeField, Range(0f, 1f)] private float songVolume = 0.8f;
    [SerializeField] private bool loopSong = true;

    [Header("Boss Health Setup")]
    private RT_BossHealth bossHealth;
    private bool phase1Active = false;
    private bool isInvulnerablePhase = false;
    [SerializeField] private int qteThreshold = 66;
    [SerializeField] private bool qteSuccessAlways = true; // testing toggle

    void Awake()
    {
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        bossSprite = GetComponent<SpriteRenderer>();
        bossHealth = GetComponent<RT_BossHealth>();
        if (bossSprite != null) originalColor = bossSprite.color;
        if (!bossHealth) Debug.LogWarning("RT_BossHealth missing on Boss.");
    }

    public void StartCutscene()
    {
        if (!playerEnteredArena)
        {
            playerEnteredArena = true;
            Debug.Log("Player entered arena. Starting boss cutscene timer...");
            StartCoroutine(BossCutsceneSequence());
        }
    }

    private void PlayBossSong()
    {
        if (!bossSong) { Debug.LogWarning("Assign bossSong in Inspector"); return; }
        audioSource.clip = bossSong;
        audioSource.volume = songVolume;
        audioSource.loop = loopSong;
        audioSource.Play();
    }

    private IEnumerator BossCutsceneSequence()
    {
        if (bossSprite) bossSprite.color = Color.magenta;
        yield return new WaitForSeconds(bossCutsceneTimer);
        if (bossSprite) bossSprite.color = originalColor;

        Debug.Log("Boss cutscene ended.");
        PlayBossSong();
        yield return Phase1AttackPattern();
    }

    #region Boss Attack Pattern Placeholders

    private IEnumerator PlayIdleAnimation()
    {
        Debug.Log("Playing Idle Animation...");
        yield return new WaitForSeconds(2f);
    }

    private IEnumerator DoFlySummonAttack()
    {
        Debug.Log("Performing Fly Summon Attack...");
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator DoBeetleSummonAttack()
    {
        Debug.Log("Performing Beetle Summon Attack...");
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator DoMachineGunRiff()
    {
        Debug.Log("Performing Machine Gun Riff...");
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator DoLongRangeAttack()
    {
        Debug.Log("Performing Long Range Attack...");
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator DoShortSwipe()
    {
        Debug.Log("Short Range: Guitar Swipe");
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator DoBigAoE()
    {
        Debug.Log("Short Range: Big AoE (projected position)");
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator DoSmallAoE()
    {
        Debug.Log("Short Range: Small AoE (lingering puddle)");
        yield return new WaitForSeconds(1f);
    }

    private void FreezePlayer() { Debug.Log("Player frozen (placeholder)"); }
    private void UnfreezePlayer() { Debug.Log("Player unfrozen (placeholder)"); }
    private void TransitionToPhase2() { Debug.Log("Phase 2 start (placeholder)"); }
    private void ResetToPhase1() { Debug.Log("Reset to Phase 1 (placeholder)"); }
    #endregion

    private IEnumerator Phase1AttackPattern()
    {
        phase1Active = true;
        isInvulnerablePhase = true;

        if (bossHealth) bossHealth.SetInvulnerable(true);
        if (bossSprite) bossSprite.color = Color.blue;

        Debug.Log("Phase 1 started - Boss is invulnerable.");

        // --- Start Pattern ---
        yield return PlayIdleAnimation();
        yield return DoFlySummonAttack();

        yield return PlayIdleAnimation();
        yield return DoMachineGunRiff();

        yield return PlayIdleAnimation();
        yield return DoLongRangeAttack();

        yield return PlayIdleAnimation();
        yield return DoMachineGunRiff();

        yield return PlayIdleAnimation();
        // --- End Pattern ---

        if (bossHealth) bossHealth.SetInvulnerable(false);
        if (bossSprite) bossSprite.color = originalColor;
        isInvulnerablePhase = false;

        // Short-range random loop until HP <= threshold, then QTE
        yield return Phase1ShortRangeLoop();

        phase1Active = false;
        Debug.Log("Invulnerability phase is over - Boss can now take damage. Starting Close Range Attacks");
    }

    private IEnumerator Phase1ShortRangeLoop()
    {
        if (!bossHealth) yield break;

        Debug.Log("Phase 1: Short-range loop started (boss is vulnerable).");

        while (bossHealth.CurrentHealth > qteThreshold)
        {
            yield return PlayIdleAnimation();

            int pick = Random.Range(0, 3); // 0,1,2
            switch (pick)
            {
                case 0: yield return DoShortSwipe(); break;
                case 1: yield return DoBigAoE(); break;
                case 2: yield return DoSmallAoE(); break;
            }
        }

        // Hit threshold -> QTE
        yield return StartQTESequence();
    }

    private IEnumerator StartQTESequence()
    {
        Debug.Log("QTE: Player frozen. Begin combo...");
        FreezePlayer();
        yield return new WaitForSeconds(2f); // placeholder QTE duration

        bool success = qteSuccessAlways; // testing
        if (success)
        {
            Debug.Log("QTE success -> Transition to Phase 2");
            UnfreezePlayer();
            TransitionToPhase2();
        }
        else
        {
            Debug.Log("QTE failed -> Reset to Phase 1");
            UnfreezePlayer();
            ResetToPhase1();
        }
    }





    //When the player enters the boss arena close the door behind them and play a cutscene
    //When the cutscene ends start phase 1

    // Phase 1 Logic
    // Play the boss song
    // Set boss HP to X (placeholder HP)
    // Start attack pattern 1

    //Attack pattern 1 logic (4 attacks)

    // Add boss invunerability
    // Play Idle Animation
    // Do FlySummonAttack
    // Play Idle animation
    // Do MachineGunRiff (This should line up to the song)
    // Play Idle animation
    // Do LongRangeAttacks (1 out of the 3 potential attacks)
    // Play Idle animation
    // MachineGunRiff (This should line up to the song)
    // Play Idle animation

    // After attack pattern 1 remove invunerability and start using CloseRangeAttacks (1 out of 3 potential attacks)
    // In between the CloseRangeAttacks play the Idle animation
    // If boss health reaches 0 stagger the boss and allow the boss to be hit to trigger a QTE event
    // 4 QTE's happen and the player needs to hit them correctly to go into phase 2
    // if they fail to hit the QTE's reset the players position to the entrance, reset boss health, and start phase 1
    // if they succeed the QTE's reset the players position to the entrance and reset boss health. Start phase 2

    // Start Phase 2

    // Phase 2 logic
    // Play the boss song
    // Set boss HP to X (placeholder HP)
    // Start attack pattern 2

    // Attack pattern 2 logic (4 attacks)

    // Add boss invunerability
    // Play Idle animation
    // Do BeetleSummonAttack
    // Play Idle animation
    // Do MachineGunRiff (This should line up to the song)
    // Play Idle animation
    // Do LongRangeAttacks (1 out of the 3 potential attacks)
    // Play Idle animation
    // MachineGunRiff (This should line up to the song)
    // Play Idle animation

    // After attack pattern 2 remove invunerability and start using CloseRangeAttacks (1 out of 3 potential attacks)
    // In between the CloseRangeAttacks play the Idle animation
    // If boss health reaches 0 stagger the boss and allow the boss to be hit to trigger a QTE event
    // 7 QTE's happen and the player needs to hit them to go into phase 3
    // if they fail to hit the QTE's reset the players position to the entrance, reset boss health, and start phase 2
    // if they succeed to hit the QTE's reset the players position to the entrance and set boss health to 1 (1 shottable)

    // Start phase 3

    // Phase 3 logic

    // Play boss song
    // Do Guitar Solo Lane Attack
    // If boss health reaches 0 stagger the boss and allow the boss to be hit to trigger a QTE event
    // 11 QTE's happen and the player needs to hit them to beat the boss
    // if they fail to hit the QTE's reset the players position to the entrance, reset boss health to 1 and restart phase 3
    // if they succeed to hit the QTE's play the boss's death cutscene


}
