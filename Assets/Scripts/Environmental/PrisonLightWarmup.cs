using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
[RequireComponent(typeof(AudioSource))]
public class PrisonLightWarmup : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The 2D light this script will control. If left empty, it grabs the Light2D on this object.")]
    public Light2D targetLight;

    [Tooltip("Buzzing or electrical hum audio source. If left empty, it grabs the AudioSource on this object.")]
    public AudioSource buzzAudio;

    [Space(8)]
    [Header("Stable Light Settings")]
    [Tooltip("The normal brightness the light settles at once it has stabilized.")]
    public float stableIntensity = 1.2f;

    [Tooltip("The normal outer radius the light settles at once it has stabilized.")]
    public float stablePointLightOuterRadius = 5f;

    [Space(8)]
    [Header("Startup Flicker")]
    [Tooltip("How long the rough startup flicker lasts before the light settles down.")]
    public float startupDuration = 2.5f;

    [Tooltip("Lowest brightness the light can dip to during startup.")]
    public float startupMinIntensity = 0.05f;

    [Tooltip("Highest brightness spike the light can reach during startup.")]
    public float startupMaxIntensity = 1.4f;

    [Tooltip("Shortest delay between startup flicker updates. Lower values feel more jittery.")]
    public float startupMinInterval = 0.03f;

    [Tooltip("Longest delay between startup flicker updates. Higher values make startup feel chunkier.")]
    public float startupMaxInterval = 0.12f;

    [Space(8)]
    [Header("Buzz Audio During Startup")]
    [Tooltip("Lowest audio pitch during startup instability.")]
    public float startupMinPitch = 0.85f;

    [Tooltip("Highest audio pitch during startup instability.")]
    public float startupMaxPitch = 1.1f;

    [Tooltip("Lowest buzz volume during startup.")]
    [Range(0f, 1f)]
    public float startupMinVolume = 0.2f;

    [Tooltip("Highest buzz volume during startup.")]
    [Range(0f, 1f)]
    public float startupMaxVolume = 0.7f;

    [Space(8)]
    [Header("Idle Flicker")]
    [Tooltip("Enables occasional subtle flicker after the light has stabilized.")]
    public bool enableIdleFlicker = true;

    [Tooltip("Minimum time before the script checks whether an idle flicker burst should happen.")]
    public float idleCheckIntervalMin = 2.5f;

    [Tooltip("Maximum time before the script checks whether an idle flicker burst should happen.")]
    public float idleCheckIntervalMax = 6f;

    [Tooltip("Chance that a flicker burst happens when checked. 0 = never, 1 = always.")]
    [Range(0f, 1f)]
    public float idleFlickerChance = 0.75f;

    [Tooltip("Minimum number of tiny flicker steps in one idle flicker burst.")]
    public int idleFlickerBurstMin = 2;

    [Tooltip("Maximum number of tiny flicker steps in one idle flicker burst.")]
    public int idleFlickerBurstMax = 4;

    [Tooltip("How far brightness can deviate from the stable value during idle flicker. Higher values are more noticeable.")]
    [Range(0f, 0.5f)]
    public float idleFlickerAmount = 0.12f;

    [Tooltip("Shortest duration of one mini flicker step during idle flicker.")]
    public float idleFlickerStepDurationMin = 0.025f;

    [Tooltip("Longest duration of one mini flicker step during idle flicker.")]
    public float idleFlickerStepDurationMax = 0.07f;

    [Space(8)]
    [Header("Optional Radius Flicker")]
    [Tooltip("Allows the light radius to shift slightly during flicker, not just the brightness.")]
    public bool flickerRadiusSlightly = true;

    [Tooltip("How much the light radius can change during flickers. Keep this low for a subtle effect.")]
    [Range(0f, 1f)]
    public float radiusFlickerAmount = 0.12f;

    [Space(8)]
    [Header("Optional Idle Sway")]
    [Tooltip("Makes the light sway gently left and right after stabilizing, as if hanging loosely.")]
    public bool enableIdleSway = true;

    [Tooltip("Horizontal sway distance from the original local position.")]
    [Range(0f, 0.25f)]
    public float swayAmount = 0.035f;

    [Tooltip("How quickly the light sways left and right.")]
    public float swaySpeed = 1.2f;

    [Tooltip("Offsets the sway timing so multiple lights do not all move in sync.")]
    public float swayPhaseOffset = 0f;

    private Coroutine flickerRoutine;
    private float baseIntensity;
    private float baseRadius;
    private Vector3 baseLocalPosition;
    private bool hasStabilized;

    private void Reset()
    {
        targetLight = GetComponent<Light2D>();
        buzzAudio = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light2D>();

        if (buzzAudio == null)
            buzzAudio = GetComponent<AudioSource>();

        baseIntensity = stableIntensity;
        baseRadius = stablePointLightOuterRadius;
        baseLocalPosition = transform.localPosition;

        if (Mathf.Approximately(swayPhaseOffset, 0f))
        {
            swayPhaseOffset = Random.Range(0f, 100f);
        }
    }

    private void OnEnable()
    {
        if (flickerRoutine != null)
            StopCoroutine(flickerRoutine);

        hasStabilized = false;
        transform.localPosition = baseLocalPosition;
        flickerRoutine = StartCoroutine(LightSequence());
    }

    private void OnDisable()
    {
        if (flickerRoutine != null)
            StopCoroutine(flickerRoutine);

        if (buzzAudio != null)
        {
            buzzAudio.Stop();
        }

        transform.localPosition = baseLocalPosition;
        hasStabilized = false;
    }

    private void Update()
    {
        if (!hasStabilized || !enableIdleSway)
            return;

        Vector3 pos = baseLocalPosition;
        pos.x += Mathf.Sin((Time.time + swayPhaseOffset) * swaySpeed) * swayAmount;
        transform.localPosition = pos;
    }

    private IEnumerator LightSequence()
    {
        // Start cold
        SetLightValues(0f, baseRadius);

        if (buzzAudio != null)
        {
            buzzAudio.loop = true;
            buzzAudio.volume = startupMinVolume;
            buzzAudio.pitch = startupMinPitch;
            buzzAudio.Play();
        }

        float elapsed = 0f;

        // Phase 1: unstable startup flicker
        while (elapsed < startupDuration)
        {
            elapsed += Random.Range(startupMinInterval, startupMaxInterval);

            float t = Mathf.Clamp01(elapsed / startupDuration);

            float currentMin = Mathf.Lerp(startupMinIntensity, stableIntensity * 0.75f, t);
            float currentMax = Mathf.Lerp(startupMaxIntensity, stableIntensity * 1.1f, t);

            float newIntensity = Random.Range(currentMin, currentMax);

            float newRadius = baseRadius;
            if (flickerRadiusSlightly)
            {
                newRadius += Random.Range(-radiusFlickerAmount, radiusFlickerAmount);
            }

            SetLightValues(newIntensity, newRadius);

            if (buzzAudio != null)
            {
                buzzAudio.pitch = Random.Range(startupMinPitch, startupMaxPitch);
                buzzAudio.volume = Mathf.Lerp(startupMaxVolume, startupMinVolume + 0.05f, t);
            }

            yield return new WaitForSeconds(Random.Range(startupMinInterval, startupMaxInterval));
        }

        // Phase 2: settle cleanly
        float settleTime = 0.35f;
        float settleElapsed = 0f;

        float startIntensity = targetLight.intensity;
        float startRadius = targetLight.pointLightOuterRadius;

        while (settleElapsed < settleTime)
        {
            settleElapsed += Time.deltaTime;
            float t = settleElapsed / settleTime;

            float newIntensity = Mathf.Lerp(startIntensity, stableIntensity, t);
            float newRadius = Mathf.Lerp(startRadius, baseRadius, t);

            SetLightValues(newIntensity, newRadius);

            if (buzzAudio != null)
            {
                buzzAudio.volume = Mathf.Lerp(buzzAudio.volume, 0.15f, t);
                buzzAudio.pitch = Mathf.Lerp(buzzAudio.pitch, 1f, t);
            }

            yield return null;
        }

        SetLightValues(stableIntensity, baseRadius);

        if (buzzAudio != null)
        {
            buzzAudio.volume = 0.15f;
            buzzAudio.pitch = 1f;
        }

        hasStabilized = true;

        // Phase 3: occasional idle flicker
        if (enableIdleFlicker)
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(idleCheckIntervalMin, idleCheckIntervalMax));

                if (Random.value <= idleFlickerChance)
                {
                    int burstCount = Random.Range(idleFlickerBurstMin, idleFlickerBurstMax + 1);

                    for (int i = 0; i < burstCount; i++)
                    {
                        float intensityOffset = Random.Range(-idleFlickerAmount, idleFlickerAmount);
                        float radiusOffset = flickerRadiusSlightly
                            ? Random.Range(-radiusFlickerAmount, radiusFlickerAmount)
                            : 0f;

                        SetLightValues(stableIntensity + intensityOffset, baseRadius + radiusOffset);

                        if (buzzAudio != null)
                        {
                            buzzAudio.pitch = Random.Range(0.965f, 1.035f);
                            buzzAudio.volume = Random.Range(0.13f, 0.18f);
                        }

                        yield return new WaitForSeconds(Random.Range(idleFlickerStepDurationMin, idleFlickerStepDurationMax));
                    }

                    SetLightValues(stableIntensity, baseRadius);

                    if (buzzAudio != null)
                    {
                        buzzAudio.pitch = 1f;
                        buzzAudio.volume = 0.15f;
                    }
                }
            }
        }
    }

    private void SetLightValues(float intensity, float radius)
    {
        if (targetLight == null)
            return;

        targetLight.intensity = Mathf.Max(0f, intensity);
        targetLight.pointLightOuterRadius = Mathf.Max(0f, radius);
    }
}