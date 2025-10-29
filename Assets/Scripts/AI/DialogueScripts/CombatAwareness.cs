using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyTagDialogue
{
    [Tooltip("The tag of the enemy this dialogue set applies to.")]
    public string enemyTag = "Enemy";

    [Header("Combat Dialogue")]
    [TextArea(2, 5)]
    public string[] combatInterruptLines;

    [TextArea(2, 5)]
    public string[] enemyDeathLines;

    [Range(0f, 1f)]
    public float deathLineChance = 1f;
}

public class CombatAwareness : MonoBehaviour
{
    [Header("Enemy Detection")]
    [Tooltip("Maximum distance to detect enemies.")]
    public float detectionRange = 10f;

    [Header("Enemy Tag Dialogue Sets")]
    public List<EnemyTagDialogue> dialogueSets = new List<EnemyTagDialogue>();

    [Header("References")]
    public FloatingDialogue floatingDialogue;

    [Header("Testing Options")]
    [Tooltip("Use the DestroyEnemy demo script for death testing.")]
    public bool useDestroyEnemyEvent = true;

    private bool enemiesNearby = false;
    private float checkInterval = 0.5f;
    private float nextCheckTime = 0f;

    private Dictionary<string, int> lastCombatIndices = new Dictionary<string, int>();
    private Dictionary<string, int> lastDeathIndices = new Dictionary<string, int>();

    public event Action<bool> OnEnemyPresenceChanged;

    private void OnEnable()
    {
        if (useDestroyEnemyEvent)
            DestroyEnemy.OnEnemyDestroyed += HandleEnemyDeathDemo;
    }

    private void OnDisable()
    {
        if (useDestroyEnemyEvent)
            DestroyEnemy.OnEnemyDestroyed -= HandleEnemyDeathDemo;
    }

    private void Update()
    {
        if (Time.time >= nextCheckTime)
        {
            UpdateEnemyPresence();
            nextCheckTime = Time.time + checkInterval;
        }
    }

    private void UpdateEnemyPresence()
    {
        bool currentState = EnemiesNearby();

        if (currentState != enemiesNearby)
        {
            enemiesNearby = currentState;
            OnEnemyPresenceChanged?.Invoke(enemiesNearby);
        }
    }

    /// <summary>
    /// Check for any enemies near this GameObject.
    /// </summary>
    public bool EnemiesNearby()
    {
        foreach (var set in dialogueSets)
        {
            if (string.IsNullOrEmpty(set.enemyTag)) continue;

            GameObject[] enemies = GameObject.FindGameObjectsWithTag(set.enemyTag);
            foreach (GameObject enemy in enemies)
            {
                if (enemy == null || !enemy.activeInHierarchy) continue;

                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance <= detectionRange)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Check for enemies near a specific position (used for object interactions).
    /// </summary>
    public bool EnemiesNearby(Vector3 position)
    {
        foreach (var set in dialogueSets)
        {
            if (string.IsNullOrEmpty(set.enemyTag)) continue;

            GameObject[] enemies = GameObject.FindGameObjectsWithTag(set.enemyTag);
            foreach (GameObject enemy in enemies)
            {
                if (enemy == null || !enemy.activeInHierarchy) continue;

                float distance = Vector3.Distance(position, enemy.transform.position);
                if (distance <= detectionRange)
                    return true;
            }
        }
        return false;
    }

    public void ShowCombatInterrupt(string enemyTag)
    {
        if (string.IsNullOrEmpty(enemyTag))
            return;

        EnemyTagDialogue set = GetDialogueSet(enemyTag);
        if (set == null || set.combatInterruptLines.Length == 0)
            return;

        int index = GetUniqueIndex(set.combatInterruptLines.Length, enemyTag, lastCombatIndices);

        if (floatingDialogue != null)
            floatingDialogue.ShowFloatingLine(set.combatInterruptLines[index]);
        else
            Debug.Log(set.combatInterruptLines[index]);
    }

    public void HandleEnemyDeath(Vector3 enemyPosition, string enemyTag)
    {
        if (string.IsNullOrEmpty(enemyTag))
            return;

        EnemyTagDialogue set = GetDialogueSet(enemyTag);
        if (set == null || set.enemyDeathLines.Length == 0)
            return;

        float distance = Vector3.Distance(transform.position, enemyPosition);
        if (distance > detectionRange) return;
        if (UnityEngine.Random.value > set.deathLineChance) return;

        int index = GetUniqueIndex(set.enemyDeathLines.Length, enemyTag, lastDeathIndices);

        if (floatingDialogue != null)
            floatingDialogue.ShowFloatingLine(set.enemyDeathLines[index]);
        else
            Debug.Log(set.enemyDeathLines[index]);

        // Refresh enemy presence immediately
        UpdateEnemyPresence();
    }

    private void HandleEnemyDeathDemo(Vector3 enemyPosition)
    {
        if (dialogueSets.Count == 0) return;
        HandleEnemyDeath(enemyPosition, dialogueSets[0].enemyTag);
    }

    private EnemyTagDialogue GetDialogueSet(string tag)
    {
        foreach (var set in dialogueSets)
        {
            if (set.enemyTag == tag)
                return set;
        }
        return null;
    }

    private int GetUniqueIndex(int length, string tag, Dictionary<string, int> lastIndices)
    {
        if (length == 1) return 0;

        int index;
        int lastIndex = lastIndices.ContainsKey(tag) ? lastIndices[tag] : -1;

        int safetyCounter = 0;
        do
        {
            index = UnityEngine.Random.Range(0, length);
            safetyCounter++;
            if (safetyCounter > 10) break;
        } while (index == lastIndex);

        lastIndices[tag] = index;
        return index;
    }

    public string GetNearestEnemyTag(Vector3 position)
    {
        float minDist = Mathf.Infinity;
        string nearestTag = null;

        foreach (var set in dialogueSets)
        {
            if (string.IsNullOrEmpty(set.enemyTag)) continue;

            GameObject[] enemies = GameObject.FindGameObjectsWithTag(set.enemyTag);
            foreach (GameObject enemy in enemies)
            {
                if (enemy == null) continue;
                float dist = Vector3.Distance(position, enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearestTag = set.enemyTag;
                }
            }
        }

        return nearestTag;
    }
}