using UnityEngine;
using System.Collections;
using NPA_Health_Components; // ✅ Added to use the Health script

public class BossAI : MonoBehaviour
{

    #region VARIABLES
    [Header("Arena Setup")]
    public float bossCutsceneTimer = 8f;
    public BoxCollider arenaTrigger;
    private bool playerEnteredArena = false;

    [Header("BossSetup")]
    private SpriteRenderer bossSprite;
    private Color originalColor;

    [Header("Boss Health Setup")]
    private Health bossHealth; // migrated from RT_BossHealth
    private bool phase1Active = false;
    private bool phase2Active = false;
    private bool isInvulnerablePhase = false;
    private bool isInvulnerable = false; // replaces SetInvulnerable()
    [SerializeField] private int qteThreshold = 60;
    [SerializeField] private bool qteSuccessAlways = true;

    [Header("Attack Setup")]
    [SerializeField] private float flashDuration = 1f;
    [SerializeField] private float flashInterval = 0.2f;

    [SerializeField] private GameObject flyEnemyPrefabA;
    [SerializeField] private GameObject flyEnemyPrefabB;
    [SerializeField] private GameObject beetleEnemyPrefabA;
    [SerializeField] private GameObject beetleEnemyPrefabB;

    [SerializeField] private Transform spawnA;
    [SerializeField] private Transform spawnB;
    [SerializeField] private Transform spawnC;
    [SerializeField] private Transform spawnD;

    /*
    [SerializeField] private ChartLoaderExample chartLoader;
    [SerializeField] private float machineGunRiffDuration = 6f;
    */

    [SerializeField] private BossStringController bossStrings;
    [SerializeField] private BossAoEThrower aoeThrower;

    [SerializeField] private GameObject swipeHitbox;
    [SerializeField] private float swipeActiveTime = 0.5f;
    [SerializeField] private int swipeDamage = 25;  // damage amount for the swipe
    [SerializeField] private Collider swipeCollider;
    [SerializeField] private Health playerHealth;   // cached reference for player health

    [SerializeField] private Transform player;
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private Transform qteStartPoint;
    [SerializeField] private Transform qteEndPoint;

    [SerializeField] private float idleAnimationDuration = 1f;



    #endregion

    void Start()
    {
        bossSprite = GetComponent<SpriteRenderer>();
        bossHealth = GetComponent<Health>(); // ✅ uses unified Health script
        if (bossSprite != null) originalColor = bossSprite.color;
        if (!bossHealth) Debug.LogWarning("Health script missing on Boss.");
        if (player)
            player.TryGetComponent(out playerHealth);

        // find player & cache their health - not sure what this does
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        if (swipeHitbox != null)
            swipeCollider = swipeHitbox.GetComponent<Collider>();
    }

    void Update()
    {
        // ✅ uses int not float
        if (Input.GetKeyDown(KeyCode.O) && bossHealth && !isInvulnerable)
            bossHealth.TakeDamage(10);
    }

    #region CUTSCENE PLACEHOLDER
    public void StartCutscene()
    {
        if (!playerEnteredArena)
        {
            playerEnteredArena = true;
            Debug.Log("Player entered arena. Starting boss cutscene timer...");
            StartCoroutine(BossCutsceneSequence());
        }
    }

    private IEnumerator BossCutsceneSequence()
    {
        if (bossSprite) bossSprite.color = Color.magenta;
        yield return new WaitForSeconds(bossCutsceneTimer);
        if (bossSprite) bossSprite.color = originalColor;

        Debug.Log("Boss cutscene ended.");
        yield return TransitionToPhase1();
    }
    #endregion CUTSCENE PLACEHOLDER

    #region BOSS ATTACK PATTERN PLACEHOLDER

    private IEnumerator PlayIdleAnimation()
    {
        Debug.Log("Playing Idle Animation...");
        yield return new WaitForSeconds(idleAnimationDuration);
    }

    private IEnumerator DoFlySummonAttack()
    {
        Debug.Log("Preparing Fly Summon...");
        yield return FlashRed(flashDuration, flashInterval);
        if (flyEnemyPrefabA && spawnA)
            Instantiate(flyEnemyPrefabA, spawnA.position, spawnA.rotation);
        if (flyEnemyPrefabB && spawnB)
            Instantiate(flyEnemyPrefabB, spawnB.position, spawnB.rotation);
        Debug.Log("Two flies summoned!");
        if (bossSprite) bossSprite.color = Color.blue;
    }

    private IEnumerator DoBeetleSummonAttack()
    {
        Debug.Log("Preparing Beetle Summon...");
        yield return FlashRed(flashDuration, flashInterval);
        if (beetleEnemyPrefabA && spawnC)
            Instantiate(beetleEnemyPrefabA, spawnC.position, spawnC.rotation);
        if (beetleEnemyPrefabB && spawnD)
            Instantiate(beetleEnemyPrefabB, spawnD.position, spawnD.rotation);
        Debug.Log("Two beetles summoned!");
        if (bossSprite) bossSprite.color = Color.blue;
    }

    private IEnumerator DoMachineGunRiff()
    {
        Debug.Log("Machine Gun Riff starting...");

        BossMachineGunRiff MGR = GetComponent<BossMachineGunRiff>();
        if (MGR == null)
        {
            Debug.LogWarning("BossMachineGunRiff component missing on boss!");
            yield break;
        }

        // Start firing (duration handled inside MGR script)
        MGR.StartMGR();

        // Wait until the internal coroutine finishes
        // (Optional: If you want BossAI to pause until it's done)
        while (true)
        {
            yield return null;
            // Break once the MGR coroutine ends (when MGR.StopMGR() sets it to null)
            var mgrField = typeof(BossMachineGunRiff)
                .GetField("MGRCoroutine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (mgrField?.GetValue(MGR) == null)
                break;
        }

        Debug.Log("Machine Gun Riff ended.");
    }

    /*
    private IEnumerator DoMachineGunRiff()
    {
        Debug.Log("Machine Gun Riff starting...");
        if (chartLoader == null)
        {
            Debug.LogWarning("ChartLoaderExample not assigned to boss!");
            yield break;
        }
        chartLoader.enabled = true;
        yield return new WaitForSeconds(machineGunRiffDuration);
        chartLoader.enabled = false;
        Debug.Log("Machine Gun Riff ended.");
    }
    */

    private IEnumerator DoLaneAttack()
    {
        Debug.Log("Starting Lane Attack...");
        if (bossStrings != null)
        {
            yield return StartCoroutine(bossStrings.BeginCombos());
        }
        else
        {
            Debug.LogWarning("BossStringController not assigned!");
        }
        Debug.Log("Lane Attack complete!");
        yield return new WaitForSeconds(1f);
    }

    private bool IsPlayerInSwipeRange()
    {
        if (swipeCollider == null || player == null) return false;

        // Grab all colliders that overlap this swipe collider’s volume
        Collider[] hits = Physics.OverlapBox(
            swipeCollider.bounds.center,
            swipeCollider.bounds.extents,
            swipeCollider.transform.rotation);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
                return true;
        }
        return false;
    }


    private IEnumerator DoShortSwipe()
    {
        Debug.Log("Boss performing short-range swipe...");
        yield return FlashRed(flashDuration, flashInterval);

        if (swipeHitbox == null || swipeCollider == null)
        {
            Debug.LogWarning("Swipe hitbox or collider not assigned.");
            yield break;
        }

        swipeHitbox.SetActive(true);
        Debug.Log("Swipe hitbox active!");

        // Small delay to let physics update
        yield return new WaitForFixedUpdate();

        // Check for player inside the collider
        if (IsPlayerInSwipeRange())
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(swipeDamage);
                Debug.Log($"Swipe hit! Player took {swipeDamage} damage!");
            }
        }

        yield return new WaitForSeconds(swipeActiveTime);
        swipeHitbox.SetActive(false);
        Debug.Log("Swipe hitbox disabled!");
    }

    private IEnumerator DoPotionThrow()
    {
        Debug.Log("Boss is preparing to throw potions...");
        PlayIdleAnimation();
        if (aoeThrower != null)
        {
            yield return StartCoroutine(aoeThrower.SpawnProjectileAndWarning());
        }
        else
        {
            Debug.LogWarning("BossAoEThrower not assigned to boss.");
        }
        Debug.Log("Potion throw complete!");
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator FlashRed(float duration, float interval)
    {
        if (!bossSprite) yield break;
        Color previousColor = bossSprite.color;
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            bossSprite.color = Color.red;
            yield return new WaitForSeconds(interval);
            bossSprite.color = previousColor;
            yield return new WaitForSeconds(interval);
        }
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
        player.position = destination.position;
    }

    private void ResetToPhase1()
    {
        Debug.Log("Reset to Phase 1 (placeholder)");
    }

    #endregion

    private IEnumerator TransitionToPhase1()
    {
        phase1Active = true;
        isInvulnerablePhase = true;
        isInvulnerable = true;

        if (bossSprite) bossSprite.color = Color.blue;

        Debug.Log("Phase 1 started - Boss is invulnerable.");

        yield return PlayIdleAnimation();
        yield return DoFlySummonAttack();

        yield return PlayIdleAnimation();
        yield return DoMachineGunRiff();

        yield return PlayIdleAnimation();
        yield return DoLaneAttack();

        yield return PlayIdleAnimation();
        yield return DoMachineGunRiff();

        yield return PlayIdleAnimation();

        isInvulnerable = false;
        if (bossSprite) bossSprite.color = originalColor;
        isInvulnerablePhase = false;

        yield return ShortRangeAttackLoop();

        if (phase2Active) yield break;

        phase1Active = false;
        Debug.Log("Invulnerability phase is over - Boss can now take damage. Starting Close Range Attacks");
    }

    private IEnumerator TransitionToPhase2()
    {
        phase2Active = true;
        isInvulnerablePhase = true;
        isInvulnerable = true;

        if (bossSprite) bossSprite.color = Color.blue;

        Debug.Log("Phase 2 started - Boss is invulnerable.");

        yield return PlayIdleAnimation();
        yield return DoBeetleSummonAttack();

        yield return PlayIdleAnimation();
        yield return DoMachineGunRiff();

        yield return PlayIdleAnimation();
        yield return DoLaneAttack();

        yield return PlayIdleAnimation();
        yield return DoMachineGunRiff();

        yield return PlayIdleAnimation();

        isInvulnerable = false;
        if (bossSprite) bossSprite.color = originalColor;
        isInvulnerablePhase = false;

        yield return ShortRangeAttackLoop();

        phase2Active = false;
        Debug.Log("Invulnerability phase is over - Boss can now take damage. Starting Close Range Attacks");
    }

    private IEnumerator ShortRangeAttackLoop()
    {
        if (!bossHealth) yield break;

        Debug.Log("Phase 1: Short-range loop started (boss is vulnerable).");

        // ✅ uses property, not private field
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

        yield return StartQTESequence();
    }

    private IEnumerator StartQTESequence()
    {
        Debug.Log("QTE: Player frozen. Begin combo...");

        if (playerController) playerController.enabled = false;
        if (player && qteStartPoint)
            yield return StartCoroutine(MovePlayerTo(qteStartPoint, 1.5f));

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

        if (player && qteEndPoint)
            yield return StartCoroutine(MovePlayerTo(qteEndPoint, 1.5f));
        if (playerController) playerController.enabled = true;

        bool success = qteSuccessAlways;

        if (success)
        {
            // ✅ uses property not private field
            if (bossHealth) bossHealth.Heal(bossHealth.MaxHealth);
            Debug.Log("QTE success → Transition to Phase 2");
            yield return StartCoroutine(TransitionToPhase2());
        }
        else
        {
            Debug.Log("QTE failed → Reset to Phase 1");
            ResetToPhase1();
        }
    }
}
