using UnityEngine;
using System.Collections;
using NPA_Health_Components;

public class BossAI : MonoBehaviour
{
    #region VARIABLES
    [Header("Arena Setup")]
    public float bossCutsceneTimer = 8f;
    public BoxCollider arenaTrigger;
    private bool playerEnteredArena = false;

    [Header("Boss Setup")]
    private SpriteRenderer bossSprite;
    private Color originalColor;

    [Header("Boss Health / Phase State")]
    private Health bossHealth;
    private bool phase1Active = false;
    private bool phase2Active = false;
    private bool isInvulnerablePhase = false;
    private bool isInvulnerable = false;
    [SerializeField] private bool qteSuccessAlways = true;

    private enum BossPhase { None, Phase1, Phase2, Phase3 }

    [Header("Telegraphing")]
    [SerializeField] private float flashDuration = 1f;
    [SerializeField] private float flashInterval = 0.2f;

    [Header("Summon Prefabs & Spawns")]
    [SerializeField] private GameObject flyEnemyPrefabA;
    [SerializeField] private GameObject flyEnemyPrefabB;
    [SerializeField] private GameObject beetleEnemyPrefabA;
    [SerializeField] private GameObject beetleEnemyPrefabB;

    [SerializeField] private Transform spawnA;
    [SerializeField] private Transform spawnB;
    [SerializeField] private Transform spawnC;
    [SerializeField] private Transform spawnD;

    [Header("Phase 3 (Chart Loader)")]
    [SerializeField] private ChartLoaderExample chartLoader;
    [SerializeField] private float chartAttackDuration = 10f;

    [Header("Other Systems")]
    [SerializeField] private BossStringController bossStrings;
    [SerializeField] private BossAoEThrower aoeThrower;

    [Header("Short Swipe / Hitbox")]
    [SerializeField] private GameObject swipeHitbox;
    [SerializeField] private float swipeActiveTime = 0.5f;
    [SerializeField] private int swipeDamage = 25;
    [SerializeField] private Collider swipeCollider;
    [SerializeField] private Health playerHealth;

    [Header("Phase QTE Threshold Percents")]
    [SerializeField] private float phase1QTEPercent = 0.66f; // 66%
    [SerializeField] private float phase2QTEPercent = 0.33f; // 33%
    [SerializeField] private float phase3QTEPercent = 0.25f; // 25% final phase

    [Header("Player / QTE")]
    [SerializeField] private Transform player;
    [SerializeField] private MonoBehaviour playerController;
    [SerializeField] private Transform qteStartPoint;
    [SerializeField] private Transform qteEndPoint;

    [SerializeField] private float idleAnimationDuration = 1f;
    #endregion

    #region LIFECYCLE
    void Start()
    {
        bossSprite = GetComponent<SpriteRenderer>();
        bossHealth = GetComponent<Health>();
        if (bossSprite) originalColor = bossSprite.color;
        if (!bossHealth) Debug.LogWarning("Health script missing on Boss.");

        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
        if (player) player.TryGetComponent(out playerHealth);

        if (swipeHitbox) swipeCollider = swipeHitbox.GetComponent<Collider>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && bossHealth && !isInvulnerable)
            bossHealth.TakeDamage(10);
    }
    #endregion

    #region CUTSCENE
    public void StartCutscene()
    {
        if (playerEnteredArena) return;
        playerEnteredArena = true;
        Debug.Log("Player entered arena. Starting boss cutscene timer...");
        StartCoroutine(BossCutsceneSequence());
    }

    private IEnumerator BossCutsceneSequence()
    {
        if (bossSprite) bossSprite.color = Color.magenta;
        yield return new WaitForSeconds(bossCutsceneTimer);
        if (bossSprite) bossSprite.color = originalColor;

        Debug.Log("Boss cutscene ended.");
        yield return TransitionToPhase1();
    }
    #endregion

    #region ATTACKS
    private IEnumerator PlayIdleAnimation()
    {
        Debug.Log("Playing Idle Animation...");
        yield return new WaitForSeconds(idleAnimationDuration);
    }

    private IEnumerator DoFlySummonAttack()
    {
        Debug.Log("Preparing Fly Summon...");
        yield return FlashRed(flashDuration, flashInterval);
        if (flyEnemyPrefabA && spawnA) Instantiate(flyEnemyPrefabA, spawnA.position, spawnA.rotation);
        if (flyEnemyPrefabB && spawnB) Instantiate(flyEnemyPrefabB, spawnB.position, spawnB.rotation);
        Debug.Log("Two flies summoned!");
        if (bossSprite) bossSprite.color = Color.blue;
    }

    private IEnumerator DoBeetleSummonAttack()
    {
        Debug.Log("Preparing Beetle Summon...");
        yield return FlashRed(flashDuration, flashInterval);
        if (beetleEnemyPrefabA && spawnC) Instantiate(beetleEnemyPrefabA, spawnC.position, spawnC.rotation);
        if (beetleEnemyPrefabB && spawnD) Instantiate(beetleEnemyPrefabB, spawnD.position, spawnD.rotation);
        Debug.Log("Two beetles summoned!");
        if (bossSprite) bossSprite.color = Color.blue;
    }

    private IEnumerator DoMachineGunRiff()
    {
        Debug.Log("Machine Gun Riff starting...");
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

        Debug.Log("Machine Gun Riff ended.");
    }

    private IEnumerator ChartLoaderAttack()
    {
        Debug.Log("Chart Loader Attack starting...");
        if (!chartLoader)
        {
            Debug.LogWarning("ChartLoaderExample not assigned to boss!");
            yield break;
        }
        chartLoader.enabled = true;
        yield return new WaitForSeconds(chartAttackDuration);
        chartLoader.enabled = false;
        Debug.Log("Chart Loader Attack ended.");
    }

    private IEnumerator DoLaneAttack()
    {
        Debug.Log("Starting Lane Attack...");
        if (bossStrings != null)
            yield return StartCoroutine(bossStrings.BeginCombos());
        else
            Debug.LogWarning("BossStringController not assigned!");

        Debug.Log("Lane Attack complete!");
        yield return new WaitForSeconds(1f);
    }

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

    private IEnumerator DoShortSwipe()
    {
        Debug.Log("Boss performing short-range swipe...");
        yield return FlashRed(flashDuration, flashInterval);

        if (!swipeHitbox || !swipeCollider)
        {
            Debug.LogWarning("Swipe hitbox or collider not assigned.");
            yield break;
        }

        swipeHitbox.SetActive(true);
        Debug.Log("Swipe hitbox active!");
        yield return new WaitForFixedUpdate();

        if (IsPlayerInSwipeRange() && playerHealth != null)
        {
            playerHealth.TakeDamage(swipeDamage);
            Debug.Log($"Swipe hit! Player took {swipeDamage} damage!");
        }

        yield return new WaitForSeconds(swipeActiveTime);
        swipeHitbox.SetActive(false);
        Debug.Log("Swipe hitbox disabled!");
    }

    private IEnumerator DoPotionThrow()
    {
        Debug.Log("Boss is preparing to throw potions...");
        yield return PlayIdleAnimation(); // (was missing 'yield return')
        if (aoeThrower != null)
            yield return StartCoroutine(aoeThrower.SpawnProjectileAndWarning());
        else
            Debug.LogWarning("BossAoEThrower not assigned to boss.");

        Debug.Log("Potion throw complete!");
        yield return new WaitForSeconds(1f);
    }

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

    #region ROUTING / QTE
    private IEnumerator TransitionToPhase(BossPhase phase)
    {
        switch (phase)
        {
            case BossPhase.Phase1: return TransitionToPhase1();
            case BossPhase.Phase2: return TransitionToPhase2();
            case BossPhase.Phase3: return TransitionToPhase3();
            default:               return null;
        }
    }

    private void ResetPhase(BossPhase phase)
    {
        switch (phase)
        {
            case BossPhase.Phase1: ResetToPhase1(); break;
            case BossPhase.Phase2: ResetToPhase2(); break;
        }
    }

    private void BossDefeated()
    {
        Debug.Log("Boss defeated!");
        // TODO: death anim, loot, disable AI, etc.
    }

    private void ResetToPhase1() { Debug.Log("Reset to Phase 1 (placeholder)"); }
    private void ResetToPhase2() { Debug.Log("Reset to Phase 2 (placeholder)"); }

    private IEnumerator StartQTESequence(BossPhase onSuccess, BossPhase onFail)
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
            if (bossHealth) bossHealth.Heal(bossHealth.MaxHealth);
            Debug.Log($"QTE success → {onSuccess}");

            if (onSuccess == BossPhase.None)
            {
                BossDefeated();
                yield break;
            }

            yield return StartCoroutine(TransitionToPhase(onSuccess));
        }
        else
        {
            Debug.Log($"QTE failed → Reset to {onFail}");
            ResetPhase(onFail);
        }
    }
    #endregion

    #region CLOSE-RANGE LOOP
    private IEnumerator ShortRangeAttackLoop(bool triggerQTEAtEnd,
                                             BossPhase qteSuccessNext = BossPhase.Phase2,
                                             BossPhase qteFailReset  = BossPhase.Phase1)
    {
        if (!bossHealth) yield break;

        // Pick correct percent by phase
        float pct = phase1Active ? phase1QTEPercent
                  : phase2Active ? phase2QTEPercent
                  :                 phase3QTEPercent;

        // Safe threshold: strictly below MaxHealth
        int threshold = Mathf.Clamp(
            Mathf.FloorToInt(bossHealth.MaxHealth * pct),
            1,
            bossHealth.MaxHealth - 1
        );

        // If we somehow start at/below threshold, refill so the loop actually runs
        if (bossHealth.CurrentHealth <= threshold)
            bossHealth.Heal(bossHealth.MaxHealth);

        Debug.Log($"[ShortLoop] pct={pct:P0} max={bossHealth.MaxHealth} threshold={threshold} current={bossHealth.CurrentHealth}");

        while (bossHealth.CurrentHealth > threshold)
        {
            yield return PlayIdleAnimation();
            switch (Random.Range(0, 2))
            {
                case 0: yield return DoShortSwipe();  break;
                case 1: yield return DoPotionThrow(); break;
            }
        }

        if (triggerQTEAtEnd)
            yield return StartQTESequence(qteSuccessNext, qteFailReset);
    }
    #endregion

    #region PHASES
    private IEnumerator TransitionToPhase1()
    {
        phase1Active = true;
        phase2Active = false;
        isInvulnerablePhase = isInvulnerable = true;

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

        // Become vulnerable
        isInvulnerable = isInvulnerablePhase = false;
        if (bossSprite) bossSprite.color = originalColor;

        // Heal + grace frame to avoid instant QTE, then loop
        if (bossHealth) bossHealth.Heal(bossHealth.MaxHealth);
        yield return null;
        yield return ShortRangeAttackLoop(true, BossPhase.Phase2, BossPhase.Phase1);

        Debug.Log("Phase 1 sequence complete.");
    }

    private IEnumerator TransitionToPhase2()
    {
        phase1Active = false;
        phase2Active = true;
        isInvulnerablePhase = isInvulnerable = true;

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

        // Become vulnerable
        isInvulnerable = isInvulnerablePhase = false;
        if (bossSprite) bossSprite.color = originalColor;

        if (bossHealth) bossHealth.Heal(bossHealth.MaxHealth);
        yield return null; // grace frame
        yield return ShortRangeAttackLoop(true, BossPhase.Phase3, BossPhase.Phase2);

        Debug.Log("Phase 2 sequence complete.");
    }

    private IEnumerator TransitionToPhase3()
    {
        Debug.Log("Phase 3 starting...");
        phase1Active = false;
        phase2Active = false;
        isInvulnerablePhase = isInvulnerable = true;
        if (bossSprite) bossSprite.color = Color.blue;

        // Invulnerable chart pattern
        yield return StartCoroutine(ChartLoaderAttack());

        // Become vulnerable for the final short window → final QTE
        isInvulnerable = isInvulnerablePhase = false;
        if (bossSprite) bossSprite.color = originalColor;

        if (bossHealth) bossHealth.Heal(bossHealth.MaxHealth);
        yield return null; // grace frame so we don't insta-QTE
        yield return ShortRangeAttackLoop(true, BossPhase.None, BossPhase.Phase3);

        Debug.Log("Phase 3 complete.");
    }
    #endregion
}
