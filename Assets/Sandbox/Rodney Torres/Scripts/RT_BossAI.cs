using UnityEngine;
using System.Collections;

public class RT_BossAI : MonoBehaviour
{
    [Header("Arena Setup")]
    public float bossCutsceneTimer = 8f;
    public BoxCollider arenaTrigger; // (unused if you use a trigger proxy)
    private bool playerEnteredArena = false;

    [Header("BossSetup")]
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
    [SerializeField] private int qteThreshold = 60;
    [SerializeField] private bool qteSuccessAlways = true; // testing toggle

    [Header("AttackSetup")]
    [SerializeField] private float flashDuration = 1f; // how long to flash total
    [SerializeField] private float flashInterval = 0.2f;    // how fast the color changes back and forth

    [SerializeField] private GameObject flyEnemyPrefabA;
    [SerializeField] private GameObject flyEnemyPrefabB;
    [SerializeField] private Transform spawnA;
    [SerializeField] private Transform spawnB;

    [SerializeField] private ChartLoaderExample chartLoader; //Chart loader script
    [SerializeField] private float machineGunRiffDuration = 6f; // how long to play before stopping

    [SerializeField] private BossStringController bossStrings;

    [SerializeField] private BossAoEThrower aoeThrower;

    [SerializeField] private GameObject swipeHitbox;
    [SerializeField] private float swipeActiveTime = 0.5f; // how long it’s active

 
    [SerializeField] private Transform player;
    [SerializeField] private MonoBehaviour playerController; // your movement script
    [SerializeField] private Transform qteStartPoint; // X
    [SerializeField] private Transform qteEndPoint;   // Y



    void Update()
    {
        //For testing QTE system
        if (Input.GetKeyDown(KeyCode.O) && bossHealth)
            bossHealth.TakeDamage(10); // press O to damage boss
    }


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

    //Placholder boss cutscene
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
        //Placeholder idle animation
        Debug.Log("Playing Idle Animation...");
        yield return new WaitForSeconds(2f);
    }

    private IEnumerator DoFlySummonAttack()
    {
        //Summon 2 fly enemies and preset locations
        Debug.Log("Preparing Fly Summon...");

        // Flash red (attack warning)
        yield return FlashRed(flashDuration, flashInterval);

        // Summon both fly enemies
        if (flyEnemyPrefabA && spawnA)
            Instantiate(flyEnemyPrefabA, spawnA.position, spawnA.rotation);

        if (flyEnemyPrefabB && spawnB)
            Instantiate(flyEnemyPrefabB, spawnB.position, spawnB.rotation);

        Debug.Log("Two flies summoned!");

        // Restore boss to invulnerability color
        if (bossSprite) bossSprite.color = Color.blue;
    }

    private IEnumerator DoBeetleSummonAttack()
    {
        Debug.Log("Performing Beetle Summon Attack...");
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator DoMachineGunRiff()
    {
        //This is placeholder until discussion of what song to put and if we should rework MGR to be something a different in this case.
        Debug.Log("Machine Gun Riff starting...");

        if (chartLoader == null)
        {
            Debug.LogWarning("ChartLoaderExample not assigned to boss!");
            yield break;
        }

        // Start the chart sequence
        chartLoader.enabled = true;

        // Let it run for the desired time
        yield return new WaitForSeconds(machineGunRiffDuration);

        // Stop it
        chartLoader.enabled = false;
        Debug.Log("Machine Gun Riff ended.");
    }

    private IEnumerator DoLaneAttack()
    {
        Debug.Log("Starting Lane Attack...");

        if (bossStrings != null)
        {
            // Correct way: directly call BeginCombos from the other script
            yield return StartCoroutine(bossStrings.BeginCombos());
        }
        else
        {
            Debug.LogWarning("BossStringController not assigned!");
        }

        Debug.Log("Lane Attack complete!");
        yield return new WaitForSeconds(1f);
    }

    /* Need the potion throw scatter shot but for now we will just do only the lane attacks for the long range attacks one instead
    private IEnumerator DoLongRangeAttack()
    {
        //Function to randomly pick and execute a long range attack
        Debug.Log("Performing Long Range Attack...");
        yield return new WaitForSeconds(1f);
    }
    */
    private IEnumerator DoShortSwipe()
    {
        Debug.Log("Boss performing short-range swipe...");

        // Flash red warning before activating the hitbox
        yield return FlashRed(flashDuration, flashInterval);

        // Instantly enable the hitbox
        if (swipeHitbox != null)
        {
            swipeHitbox.SetActive(true);
            Debug.Log("Swipe hitbox active!");
            yield return new WaitForSeconds(swipeActiveTime);
            swipeHitbox.SetActive(false);
            Debug.Log("Swipe hitbox disabled!");
        }
    }

    private IEnumerator DoPotionThrow()
    {
        Debug.Log("Boss is preparing to throw potions...");

        PlayIdleAnimation();

        if (aoeThrower != null)
        {
            // Manually trigger the AoE throw sequence once
            yield return StartCoroutine(aoeThrower.SpawnProjectileAndWarning());
        }
        else
        {
            Debug.LogWarning("BossAoEThrower not assigned to boss.");
        }

        Debug.Log("Potion throw complete!");
        yield return new WaitForSeconds(1f); // small delay for rhythm
    }

    private IEnumerator FlashRed(float duration, float interval)
    {
        if (!bossSprite) yield break;

        // Save whatever color the boss currently has
        Color previousColor = bossSprite.color;
        float startTime = Time.time;

        // Alternate between red and the previous color
        while (Time.time - startTime < duration)
        {
            bossSprite.color = Color.red;
            yield return new WaitForSeconds(interval);
            bossSprite.color = previousColor;
            yield return new WaitForSeconds(interval);
        }

        // End on the original color
        bossSprite.color = previousColor;
    }

    private IEnumerator MovePlayerTo(Transform destination, float duration)
    {
        if (!player || !destination) yield break;

        Vector3 start = player.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            player.position = Vector3.Lerp(start, destination.position, t);
            yield return null;
        }

        player.position = destination.position; // snap to exact end
    }

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

        //Placeholder until I can get the random LongRangeAttack Implemented
        yield return PlayIdleAnimation();
        yield return DoLaneAttack();
        //Placeholder until I can get the random LongRangeAttack Implemented

        /*
        yield return PlayIdleAnimation();
        yield return DoLongRangeAttack();
        */

        yield return PlayIdleAnimation();
        yield return DoMachineGunRiff();

        yield return PlayIdleAnimation();
        // --- End Pattern ---

        if (bossHealth) bossHealth.SetInvulnerable(false);
        if (bossSprite) bossSprite.color = originalColor;
        isInvulnerablePhase = false;

        // Short-range random loop until HP <= threshold, then QTE
        yield return ShortRangeAttackLoop();

        phase1Active = false;
        Debug.Log("Invulnerability phase is over - Boss can now take damage. Starting Close Range Attacks");
    }

    private IEnumerator ShortRangeAttackLoop()
    {
        //Function to randomly pick and execute a short range attack
        if (!bossHealth) yield break;

        Debug.Log("Phase 1: Short-range loop started (boss is vulnerable).");

        while (bossHealth.CurrentHealth > qteThreshold)
        {
            yield return PlayIdleAnimation();

            int pick = Random.Range(0, 2);
            switch (pick)
            {
                case 0: yield return DoShortSwipe(); break;
                case 1: yield return DoPotionThrow(); break;
            }
        }

        // Hit threshold -> QTE
        yield return StartQTESequence();
    }

    private IEnumerator StartQTESequence()
    {
        Debug.Log("QTE: Player frozen. Begin combo...");

        // Freeze player controller and move to QTE start point
        if (playerController) playerController.enabled = false;
        if (player && qteStartPoint)
            yield return StartCoroutine(MovePlayerTo(qteStartPoint, 1.5f)); 

        // Start a 4-note QTE this was done by limiting the array to only 4. I didnt want to change the other script since it picks a random set from the array
        if (QTEManager.Instance != null)
        {
            QTEManager.Instance.StartQTE();
            yield return new WaitUntil(() => !QTEManager.Instance.IsActive);
        }
        else
        {
            Debug.LogWarning("QTEManager.Instance not found.");
            yield break;
        }

        //Move to arena start and unfreeze player controller
        if (player && qteEndPoint)
            yield return StartCoroutine(MovePlayerTo(qteEndPoint, 1.5f)); 
        if (playerController) playerController.enabled = true;

        // For now, use your testing flag to branch
        bool success = qteSuccessAlways;
        if (success)
        {
            Debug.Log("QTE success → Transition to Phase 2");
            TransitionToPhase2();
        }
        else
        {
            Debug.Log("QTE failed → Reset to Phase 1");
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
