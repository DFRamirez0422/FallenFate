using NPA_Health_Components;
using NPA_PlayerPrefab.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossAI : MonoBehaviour
{
    //Enum for our boss phase transition logic. The QTE system depends on this know if what to do if the player fails or succeeds the QTE's at the end of each phase
    private enum BossPhase { None, Phase1, Phase2, Phase3 }

    #region VARIABLES
    //Working good this setups up a box collider that starts the boss cutscene which we dont have one so I turned him purple for a editable amount of time
    [Header("Arena Setup")]
    public float bossCutsceneTimer = 8f;
    public BoxCollider arenaTrigger;
    private bool playerEnteredArena = false;

    //These are for changing the color to signify the status of the boss since we dont have animations. 
    [Header("Boss Setup")]
    private SpriteRenderer bossSprite;
    private Color originalColor;

    //Variables for health.
    [Header("Boss Health / Phase State")]
    private EnemyHP bossHealth;

    //Variables to give visual warning the boss is going to attack. This is just flashing red
    [Header("Telegraphing")]
    [SerializeField] private float flashDuration = 1f;
    [SerializeField] private float flashInterval = 0.2f;

    //Variables for summoning enemies
    [Header("Summon Prefabs & Spawns")]
    [SerializeField] private GameObject flyEnemyPrefabA;
    [SerializeField] private GameObject flyEnemyPrefabB;
    [SerializeField] private GameObject beetleEnemyPrefabA;
    [SerializeField] private GameObject beetleEnemyPrefabB;

    [SerializeField] private Transform spawnA;
    [SerializeField] private Transform spawnB;
    [SerializeField] private Transform spawnC;
    [SerializeField] private Transform spawnD;

    //Variables for the chart loader script
    [Header("Chart Loader Variables")]
    [SerializeField] private ChartLoaderExample chartLoader;
    [SerializeField] private float chartAttackDuration = 10f;

    // These are variables needed to do the long range attacks. Which are the lane attack and the potion throws.
    [Header("Long Range Attack Variables")]
    [SerializeField] private BossStringController bossStrings;
    [SerializeField] private BossAoEThrower aoeThrower;

    //Variables for the boss to do a short swipe. This is spawning a simple hit box in front of him.
    [Header("Short Swipe / Hitbox")]
    [SerializeField] private GameObject swipeHitbox;
    [SerializeField] private float swipeActiveTime = 0.5f;
    [SerializeField] private int swipeDamage = 25;
    [SerializeField] private Collider swipeCollider;
    [SerializeField] private Health playerHealth;

    //Variables in relation to the QTE success, failuer, and trigger conditions
    [Header("Phase QTE Threshold Percents")]
    [SerializeField] private float phase1QTEPercent = 0.66f; // 66%
    [SerializeField] private float phase2QTEPercent = 0.33f; // 33%
    [SerializeField] private float phase3QTEPercent = 0.25f; // 25% final phase
    private bool phase1Active = false;
    private bool phase2Active = false;
    private bool isInvulnerable = false;
    public bool IsInvulnerable => isInvulnerable; //Encapsalation for other scripts to check if the boss is invulnerable
    [SerializeField] private bool qteSuccessAlways = true;

    //Variables for transporting the player to the desired location after QTE's fails or succeeds
    [Header("Player / QTE")]
    [SerializeField] private Transform player;
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private Transform qteStartPoint;
    [SerializeField] private Transform qteEndPoint;

    //A variable for the time the boss will idle but this is placeholder since we dont have animations
    [SerializeField] private float idleAnimationDuration = 1f;
    #endregion

    #region INITITALIZATION AND VALIDATION

    //We initialize the sprite renderer for changing the color for placeholder warning and indicators.
    //We initialize the health script and we validate it in case it doesnt work.
    //We also check if the player is valid and we store its transform in a variable called player. We then get the players health component if the player is valid.
    //We check if the swipe hit box is valid and we get its collider and store it in a variable
    void Start()
    {
        bossSprite = GetComponent<SpriteRenderer>();
        bossHealth = GetComponent<EnemyHP>();
        if (bossSprite) originalColor = bossSprite.color;
        if (!bossHealth) Debug.LogWarning("EnemyHP script missing on Boss.");

        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
        if (player) player.TryGetComponent(out playerHealth);

        if (player && playerCombat == null)
            player.TryGetComponent(out playerCombat);

        if (swipeHitbox) swipeCollider = swipeHitbox.GetComponent<Collider>();
    }

    //For testing damage threshold triggers while the boss cant be damaged during development
    void Update()
    {
       // if (Input.GetKeyDown(KeyCode.O) && bossHealth && !isInvulnerable) bossHealth.TakeDamage(10);
    }
    #endregion

    #region CUTSCENE
    //This function checks if the bool that gets triggered true when the player enters the arena box collider is true then it starts the boss cutscene function.
    public void StartCutscene()
    {
        if (playerEnteredArena) return;
        playerEnteredArena = true;
        // Debug.Log("Player entered arena. Starting boss cutscene timer...");
        StartCoroutine(BossCutsceneSequence());
    }

    //This gets the spritecomponent and sets it purple for the duration to signify the cutscene happening. Then it returns it to its original color and starts phase 1.
    private IEnumerator BossCutsceneSequence()
    {
        if (bossSprite) bossSprite.color = Color.magenta;
        yield return new WaitForSeconds(bossCutsceneTimer);
        if (bossSprite) bossSprite.color = originalColor;

        // Debug.Log("Boss cutscene ended.");
        yield return TransitionToPhase1();
    }
    #endregion

    #region ATTACKS
    //This is a placeholder timer for idle animations. I put this in in between attacks to give the boss a more natural feel while developing
    private IEnumerator PlayIdleAnimation()
    {
        // Debug.Log("Playing Idle Animation...");
        yield return new WaitForSeconds(idleAnimationDuration);
    }

    //This is the fly summon attack. It does a warning flash then it creates the enemy prefabs at the desired location if they are valid in the inspector.
    private IEnumerator DoFlySummonAttack()
    {
        // Debug.Log("Preparing Fly Summon...");
        yield return FlashRed(flashDuration, flashInterval);
        if (flyEnemyPrefabA && spawnA) Instantiate(flyEnemyPrefabA, spawnA.position, spawnA.rotation);
        if (flyEnemyPrefabB && spawnB) Instantiate(flyEnemyPrefabB, spawnB.position, spawnB.rotation);
        // Debug.Log("Two flies summoned!");
        if (bossSprite) bossSprite.color = Color.blue;
    }

    //This is the beetle summon attack. It does a warning flash then it creates the enemy prefabs at the desired location if they are valid in the inspector.
    private IEnumerator DoBeetleSummonAttack()
    {
        // Debug.Log("Preparing Beetle Summon...");
        yield return FlashRed(flashDuration, flashInterval);
        if (beetleEnemyPrefabA && spawnC) Instantiate(beetleEnemyPrefabA, spawnC.position, spawnC.rotation);
        if (beetleEnemyPrefabB && spawnD) Instantiate(beetleEnemyPrefabB, spawnD.position, spawnD.rotation);
        // Debug.Log("Two beetles summoned!");
        if (bossSprite) bossSprite.color = Color.blue;
    }

    //This function validates the MGR component and does the MGR mechanic from another script and activates it. This shoots a ton of projectiles at the players location.
    private IEnumerator DoMachineGunRiff()
    {
        // Debug.Log("Machine Gun Riff starting...");
        var MGR = GetComponent<BossMachineGunRiff>();
        if (MGR == null)
        {
            Debug.LogWarning("BossMachineGunRiff component missing on boss!");
            yield break;
        }

        MGR.StartMGR();

        while (true)
        {
            yield return null;
            var mgrField = typeof(BossMachineGunRiff)
                .GetField("MGRCoroutine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (mgrField?.GetValue(MGR) == null) break;
        }

        //  Debug.Log("Machine Gun Riff ended.");
    }

    //This function starts the chart loader attack if its valid. The total of time it does it is editable. This was working using angle on the fire directions.
    private IEnumerator ChartLoaderAttack()
    {
        // Debug.Log("Chart Loader Attack starting...");
        if (!chartLoader)
        {
            Debug.LogWarning("ChartLoaderExample not assigned to boss!");
            yield break;
        }
        chartLoader.enabled = true;
        yield return new WaitForSeconds(chartAttackDuration);
        chartLoader.enabled = false;
        // Debug.Log("Chart Loader Attack ended.");
    }

    //This function starts the lane attack from another script if its valid.
    private IEnumerator DoLaneAttack()
    {
        // Debug.Log("Starting Lane Attack...");
        if (bossStrings != null)
            yield return StartCoroutine(bossStrings.BeginCombos());
        else
            Debug.LogWarning("BossStringController not assigned!");

        //  Debug.Log("Lane Attack complete!");
        yield return new WaitForSeconds(1f);
    }

    //This function checks if the collider for the shortrangeswipe overlaps with the player and then returns true if it does. It validates both as well early.
    private bool IsPlayerInSwipeRange()
    {
        if (!swipeCollider || !player) return false;

        Collider[] hits = Physics.OverlapBox(
            swipeCollider.bounds.center,
            swipeCollider.bounds.extents,
            swipeCollider.transform.rotation);

        foreach (var h in hits)
            if (h.CompareTag("Player")) return true;

        return false;
    }

    //This function does a shortrange swipe. which is just spawning a box collider in front of the boss with a placeholder warning. It validates the box and player.
    private IEnumerator DoShortSwipe()
    {
        // Debug.Log("Boss performing short-range swipe...");
        yield return FlashRed(flashDuration, flashInterval);

        if (!swipeHitbox || !swipeCollider)
        {
            Debug.LogWarning("Swipe hitbox or collider not assigned.");
            yield break;
        }

        swipeHitbox.SetActive(true);
        //Debug.Log("Swipe hitbox active!");
        yield return new WaitForFixedUpdate();

        if (IsPlayerInSwipeRange() && playerHealth != null)
        {
            playerHealth.TakeDamage(swipeDamage);
            Debug.Log($"Swipe hit! Player took {swipeDamage} damage!");
        }

        yield return new WaitForSeconds(swipeActiveTime);
        swipeHitbox.SetActive(false);
        // Debug.Log("Swipe hitbox disabled!");
    }

    //This function is supposed to do the potion throw mechanics but the mechanic has not finished being developed. So this placeholder.
    private IEnumerator DoPotionThrow()
    {
       // Debug.Log("Boss is preparing to throw potions...");
        yield return PlayIdleAnimation();
        if (aoeThrower != null)
            yield return StartCoroutine(aoeThrower.SpawnProjectileAndWarning());
        else
            // Debug.LogWarning("BossAoEThrower not assigned to boss.");

        // Debug.Log("Potion throw complete!");
        yield return new WaitForSeconds(1f);
    }

    //This function flashes the boss red and its previous color to indicate its going to use an attack. Its placeholder for the animations.
    private IEnumerator FlashRed(float duration, float interval)
    {
        if (!bossSprite) yield break;
        Color prev = bossSprite.color;
        float start = Time.time;
        while (Time.time - start < duration)
        {
            bossSprite.color = Color.red;
            yield return new WaitForSeconds(interval);
            bossSprite.color = prev;
            yield return new WaitForSeconds(interval);
        }
        bossSprite.color = prev;
    }

    //This function translates the player position to a desired location over a editable time.
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
        player.position = destination.position;
    }
    #endregion

    #region QTE and PHASE TRANSITION LOGIC
    //This function uses enums to decide what phase to start
    private IEnumerator TransitionToPhase(BossPhase phase)
    {
        switch (phase)
        {
            case BossPhase.Phase1: return TransitionToPhase1();
            case BossPhase.Phase2: return TransitionToPhase2();
            case BossPhase.Phase3: return TransitionToPhase3();
            default: return null;
        }
    }
    //If you fail the QTE this function will be called to reset the phase.
    private void ResetPhase(BossPhase phase)
    {
        switch (phase)
        {
            case BossPhase.Phase1:
                StartCoroutine(TransitionToPhase1());
                break;

            case BossPhase.Phase2:
                StartCoroutine(TransitionToPhase2());
                break;

            case BossPhase.Phase3:
                StartCoroutine(TransitionToPhase3());
                break;
        }
    }


    //This function is called upon the defeat of the boss after phase 3's QTE is succeeded. I want it load a different scene currently.
    private void BossDefeated()
    {
        Debug.Log("Boss defeated!");
        SceneManager.LoadScene("Credits");
        //Load scene : credits
        // TODO: death anim, loot, disable AI, etc.
    }

    //This function is tricky so I will outline so I can udnerstand it
    private IEnumerator StartQTESequence(BossPhase onSuccess, BossPhase onFail)
    {
        Debug.Log("QTE: Player frozen. Begin combo...");


        if (playerController) playerController.enabled = false; // Validate the player controller and set it to false.
        if (playerCombat) playerCombat.enabled = false; // Validate and disable player combat here
        //TODO: We also need to disable the players combat since while im being translated I can still do attacks.

        if (player && qteStartPoint) // validate the player position and the translation position
            yield return StartCoroutine(MovePlayerTo(qteStartPoint, 1.5f)); //Move the player to the position

        if (QTEManager.Instance != null) // Validate the QTEManager instance. Could probably delete the != null
        {
            int presetIndex = 0;

            if (phase1Active)
                presetIndex = 0;   // uses spawnSet[0] → 4 notes
            else if (phase2Active)
                presetIndex = 1;   // uses spawnSet[1] → 7 notes
            else
                presetIndex = 2;   // phase 3 / fallback → spawnSet[2] → 11 notes

            QTEManager.Instance.StartQTEWithPresetIndex(presetIndex);
            yield return new WaitUntil(() => !QTEManager.Instance.IsActive);
        }
        else //break if you cant do the QTE sequence
        {
            Debug.LogWarning("QTEManager.Instance not found.");
            yield break;
        }

        //Move the player to the position except for in phase 3 success. In that case I dont want to.
        if (player && qteEndPoint && onSuccess != BossPhase.None)
            yield return StartCoroutine(MovePlayerTo(qteEndPoint, 1.5f)); 

        if (playerController) playerController.enabled = true; // re enable the player controller and should be reenabling the player combat.
        if (playerCombat) playerCombat.enabled = true; // Validate and reenable player combat here as well.

        // bool success = qteSuccessAlways; // This makes me always succeed during development which I want to get rid of.
        bool success;

        if (qteSuccessAlways || QTEManager.Instance == null)
        {
            // Dev override or no manager → always succeed
            success = true;
        }
        else
        {
            var qte = QTEManager.Instance;

            int partials = qte.PartialCount;
            int misses = qte.MissCount;

            bool failDueToPartials = partials > qte.partialFailThreshold;
            bool failDueToMisses = misses >= qte.missFailThreshold;

            success = !(failDueToPartials || failDueToMisses);
        }


        //Currently I dont have a way of checking if I failed or not. I just have it always set to true.
        //TODO: make the qteSuccessAlways into qteOutcome and have that be success or failure
        if (success)
        {
            if (bossHealth) bossHealth.currentHealth = bossHealth.maxHealth; 
            Debug.Log($"QTE success → {onSuccess}");

            if (onSuccess == BossPhase.None)
            {
                BossDefeated();
                yield break;
            }

            yield return StartCoroutine(TransitionToPhase(onSuccess));
        }
        else // this else case should set the phase enum  to what I want it to be. So if its phase 1 I set the enum to phase 1 and that should transition me back to phase 1
        {
            Debug.Log($"QTE failed → Reset to {onFail}");

            // For Phase 3 fails, also move the player back to arena start
            if (onFail == BossPhase.Phase3 && player && qteEndPoint)
                yield return StartCoroutine(MovePlayerTo(qteEndPoint, 1.5f));

            ResetPhase(onFail);
        }
    }
    #endregion

    #region CLOSE-RANGE LOOP
    //This function is more complicated then its needs to be because it holds information on how to transition phases.
    //Basically the goal was to randomly select between 2 attacks but during this loop it checks its own hp and if you reach the threshold depending on the phase
    //You translate the player to a desired location and start the QTE. If you succeed or fail you get translated to the start of the arena except for phase 3 where you stay.
    //Then depending on if you did the QTE's good you transition phases or you restart phases.

    //I dont undertsand the parameters of this function and their purpose yet.
    private IEnumerator ShortRangeAttackLoop(bool triggerQTEAtEnd,
                                             BossPhase qteSuccessNext = BossPhase.Phase2,
                                             BossPhase qteFailReset = BossPhase.Phase1) // I think these params being set to = are too restrictive and might need to get changed.
    {
        if (!bossHealth) yield break; // early break if boss health is not valid

        // Pick correct percent by phase
        float pct = phase1Active ? phase1QTEPercent
                  : phase2Active ? phase2QTEPercent
                  : phase3QTEPercent; // the variable name pct does not make sense but the rest does. Depending on the bool it will pick the float pct.

        // Safe threshold: strictly below MaxHealth
        int threshold = Mathf.Clamp(
            Mathf.FloorToInt(bossHealth.maxHealth * pct),
            1,
            bossHealth.maxHealth - 1

        //I want a debug message showing what the threshold is when it is set.
        );

        // If we somehow start at/below threshold, refill so the loop actually runs
        if (bossHealth.currentHealth <= threshold)
            if (bossHealth) bossHealth.currentHealth = bossHealth.maxHealth; // need to replace heal with something else thats simpler like bossHealth = bossMaxHealth. Calling this script is gonna change

       // Debug.Log($"[ShortLoop] pct={pct:P0} max={bossHealth.maxHealth} threshold={threshold} current={bossHealth.CurrentHealth}"); // not sure if this works.

        while (bossHealth.currentHealth > threshold)
        {
            yield return PlayIdleAnimation();
            switch (Random.Range(0, 2))
            {
                case 0: yield return DoShortSwipe(); break;
                case 1: yield return DoPotionThrow(); break;
            }
        }
        //This is what triggers the start of the qte sequence. Im not sure if triggerQTEAtEnd is even needed
        if (triggerQTEAtEnd)
            yield return StartQTESequence(qteSuccessNext, qteFailReset);
    }
    #endregion

    #region PHASES
    private IEnumerator TransitionToPhase1()
    {
        phase1Active = true; // for picking pct variable
        phase2Active = false; // for picking pct variable
        isInvulnerable = true; // For invulnerability phase

        if (bossSprite) bossSprite.color = Color.blue; // Placeholder sprite color change for invunerability immunity indicator
                                                       // Debug.Log("Phase 1 started - Boss is invulnerable.");

        // yield return StartCoroutine(ChartLoaderAttack()); // For testing since it takes a while to get to phase 3
        // BossDefeated(); // for testing the scene transition
        // yield return StartCoroutine(TransitionToPhase3()); // For testing: skip Phase 1 and go straight to Phase 3

        yield return PlayIdleAnimation();
        yield return DoFlySummonAttack();

        yield return PlayIdleAnimation();
        yield return DoMachineGunRiff();

        yield return PlayIdleAnimation();
        yield return DoLaneAttack();

        yield return PlayIdleAnimation();
        yield return DoMachineGunRiff();

        yield return PlayIdleAnimation();

        // Disable invunerability indicator and change to original color to indicate the immunity is off
        isInvulnerable = false;
        if (bossSprite) bossSprite.color = originalColor;

        // Heal + grace frame to avoid instant QTE, then loop
        if (bossHealth) bossHealth.currentHealth = bossHealth.maxHealth; // This should be replaced to a simple bossHealth = bossMaxHealth
        yield return null;
        yield return ShortRangeAttackLoop(true, BossPhase.Phase2, BossPhase.Phase1); // not sure what these params mean but they are prob the success and failure reqs.

        // Debug.Log("Phase 1 sequence complete.");
    }

    private IEnumerator TransitionToPhase2()
    {
        phase1Active = false;
        phase2Active = true;
        isInvulnerable = true;

        if (bossSprite) bossSprite.color = Color.blue;
        // Debug.Log("Phase 2 started - Boss is invulnerable.");

        yield return PlayIdleAnimation();
        yield return DoBeetleSummonAttack();

        yield return PlayIdleAnimation();
        yield return DoMachineGunRiff();

        yield return PlayIdleAnimation();
        yield return DoLaneAttack();

        yield return PlayIdleAnimation();
        yield return DoMachineGunRiff();

        yield return PlayIdleAnimation();

        // Become vulnerable
        isInvulnerable = false;
        if (bossSprite) bossSprite.color = originalColor;

        if (bossHealth) bossHealth.currentHealth = bossHealth.maxHealth;
        yield return null; // grace frame
        yield return ShortRangeAttackLoop(true, BossPhase.Phase3, BossPhase.Phase2);

        // Debug.Log("Phase 2 sequence complete.");
    }

    private IEnumerator TransitionToPhase3()
    {
       // Debug.Log("Phase 3 starting...");
        phase1Active = false;
        phase2Active = false;
        isInvulnerable = true;
        if (bossSprite) bossSprite.color = Color.blue;

        // Invulnerable chart pattern
        yield return StartCoroutine(ChartLoaderAttack());

        // Become vulnerable for the final short window → final QTE
        isInvulnerable = false;
        if (bossSprite) bossSprite.color = originalColor;

        if (bossHealth) bossHealth.currentHealth = bossHealth.maxHealth;
        yield return null; // grace frame so we don't insta-QTE
        yield return ShortRangeAttackLoop(true, BossPhase.None, BossPhase.Phase3);

        // Debug.Log("Phase 3 complete.");
    }
    #endregion
}
