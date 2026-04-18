using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FickerObjectSpawner : MonoBehaviour
{
    public enum TriggerAction
    {
        Spawn,
        Despawn
    }

    [Header("Objects to toggle on player enter")]
    [Tooltip("Objects affected when the player enters this trigger.")]
    public List<GameObject> targetObjects = new List<GameObject>();

    [Header("Action to apply on enter")]
    [Tooltip("Spawn = turn objects on. Despawn = turn objects off.")]
    public TriggerAction action = TriggerAction.Spawn;

    [Header("Trigger Behavior")]
    [Tooltip("If true, this trigger only works once.")]
    public bool triggerOnlyOnce = true;

    [Tooltip("Optional delay before applying the action.")]
    public float actionDelay = 0f;

    [Header("Spawn Flicker")]
    [Tooltip("If true, spawned objects flicker into existence instead of appearing instantly.")]
    public bool useFlickerSpawn = true;

    [Tooltip("How long the flicker-in lasts. Lower = faster overall spawn.")]
    public float spawnFlickerDuration = 0.35f;

    [Tooltip("Shortest delay between flicker steps during spawn.")]
    public float spawnFlickerMinInterval = 0.015f;

    [Tooltip("Longest delay between flicker steps during spawn.")]
    public float spawnFlickerMaxInterval = 0.04f;

    [Header("Despawn Flicker")]
    [Tooltip("If true, despawned objects flicker out instead of disappearing instantly.")]
    public bool useFlickerDespawn = true;

    [Tooltip("How long the flicker-out lasts. Lower = faster overall despawn.")]
    public float despawnFlickerDuration = 0.4f;

    [Tooltip("Shortest delay between flicker steps during despawn.")]
    public float despawnFlickerMinInterval = 0.015f;

    [Tooltip("Longest delay between flicker steps during despawn.")]
    public float despawnFlickerMaxInterval = 0.05f;

    [Header("Gameplay Safety During Flicker")]
    [Tooltip("When despawning, disable colliders right away so the object stops interacting before it visually disappears.")]
    public bool disableCollidersImmediatelyOnDespawn = true;

    [Tooltip("When spawning, keep colliders disabled until flicker-in is finished.")]
    public bool keepCollidersDisabledUntilSpawnFinishes = true;

    [Tooltip("When despawning, disable animators right away.")]
    public bool disableAnimatorsImmediatelyOnDespawn = false;

    [Tooltip("When spawning, keep animators disabled until flicker-in is finished.")]
    public bool keepAnimatorsDisabledUntilSpawnFinishes = false;

    private bool hasTriggered = false;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered && triggerOnlyOnce)
            return;

        if (!collision.CompareTag("Player"))
            return;

        hasTriggered = true;
        StartCoroutine(HandleTrigger());
    }

    private IEnumerator HandleTrigger()
    {
        if (actionDelay > 0f)
        {
            yield return new WaitForSeconds(actionDelay);
        }

        List<Coroutine> runningEffects = new List<Coroutine>();

        for (int i = 0; i < targetObjects.Count; i++)
        {
            GameObject target = targetObjects[i];

            if (target == null)
                continue;

            if (action == TriggerAction.Spawn)
            {
                if (useFlickerSpawn)
                {
                    Coroutine routine = StartCoroutine(FlickerSpawnObject(target));
                    runningEffects.Add(routine);
                }
                else
                {
                    target.SetActive(true);
                }
            }
            else
            {
                if (useFlickerDespawn)
                {
                    Coroutine routine = StartCoroutine(FlickerDespawnObject(target));
                    runningEffects.Add(routine);
                }
                else
                {
                    target.SetActive(false);
                }
            }
        }

        // Wait roughly long enough for the whole group effect to finish.
        if (action == TriggerAction.Spawn && useFlickerSpawn)
        {
            yield return new WaitForSeconds(spawnFlickerDuration + spawnFlickerMaxInterval);
        }
        else if (action == TriggerAction.Despawn && useFlickerDespawn)
        {
            yield return new WaitForSeconds(despawnFlickerDuration + despawnFlickerMaxInterval);
        }
    }

    private IEnumerator FlickerSpawnObject(GameObject target)
    {
        if (target == null)
            yield break;

        target.SetActive(true);

        SpriteRenderer[] spriteRenderers = target.GetComponentsInChildren<SpriteRenderer>(true);
        Collider2D[] colliders = target.GetComponentsInChildren<Collider2D>(true);
        Animator[] animators = target.GetComponentsInChildren<Animator>(true);

        if (keepCollidersDisabledUntilSpawnFinishes)
        {
            SetCollidersEnabled(colliders, false);
        }

        if (keepAnimatorsDisabledUntilSpawnFinishes)
        {
            SetAnimatorsEnabled(animators, false);
        }

        // Start invisible so they flicker in.
        SetRenderersVisible(spriteRenderers, false);

        float elapsed = 0f;
        bool visible = false;

        while (elapsed < spawnFlickerDuration)
        {
            float waitTime = Random.Range(spawnFlickerMinInterval, spawnFlickerMaxInterval);
            elapsed += waitTime;

            visible = !visible;
            SetRenderersVisible(spriteRenderers, visible);

            yield return new WaitForSeconds(waitTime);
        }

        SetRenderersVisible(spriteRenderers, true);

        if (keepCollidersDisabledUntilSpawnFinishes)
        {
            SetCollidersEnabled(colliders, true);
        }

        if (keepAnimatorsDisabledUntilSpawnFinishes)
        {
            SetAnimatorsEnabled(animators, true);
        }
    }

    private IEnumerator FlickerDespawnObject(GameObject target)
    {
        if (target == null || !target.activeInHierarchy)
            yield break;

        SpriteRenderer[] spriteRenderers = target.GetComponentsInChildren<SpriteRenderer>(true);
        Collider2D[] colliders = target.GetComponentsInChildren<Collider2D>(true);
        Animator[] animators = target.GetComponentsInChildren<Animator>(true);

        if (disableCollidersImmediatelyOnDespawn)
        {
            SetCollidersEnabled(colliders, false);
        }

        if (disableAnimatorsImmediatelyOnDespawn)
        {
            SetAnimatorsEnabled(animators, false);
        }

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < despawnFlickerDuration)
        {
            float waitTime = Random.Range(despawnFlickerMinInterval, despawnFlickerMaxInterval);
            elapsed += waitTime;

            visible = !visible;
            SetRenderersVisible(spriteRenderers, visible);

            yield return new WaitForSeconds(waitTime);
        }

        SetRenderersVisible(spriteRenderers, false);
        target.SetActive(false);
    }

    private void SetRenderersVisible(SpriteRenderer[] renderers, bool visible)
    {
        if (renderers == null)
            return;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].enabled = visible;
            }
        }
    }

    private void SetCollidersEnabled(Collider2D[] colliders, bool enabledState)
    {
        if (colliders == null)
            return;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                colliders[i].enabled = enabledState;
            }
        }
    }

    private void SetAnimatorsEnabled(Animator[] animators, bool enabledState)
    {
        if (animators == null)
            return;

        for (int i = 0; i < animators.Length; i++)
        {
            if (animators[i] != null)
            {
                animators[i].enabled = enabledState;
            }
        }
    }
}