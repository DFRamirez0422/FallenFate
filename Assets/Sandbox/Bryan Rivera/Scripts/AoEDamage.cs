using UnityEngine;
using System.Collections.Generic;

public class AoEDamage : MonoBehaviour
{
    public int damage = 5;
    public float lifeTime = 2f;
    public bool damageOnce = true;
    public float repeatDamageInterval = 0.5f; // Only for damageOnce = false

    private HashSet<GameObject> damagedOnce = new HashSet<GameObject>();
    private Dictionary<GameObject, float> nextDamageTime = new Dictionary<GameObject, float>();

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        var health = other.GetComponent<NPA_Health_Components.Health>();
        if (health == null) return;

        if (damageOnce)
        {
            if (damagedOnce.Contains(other.gameObject)) return;

            health.TakeDamage(damage);
            damagedOnce.Add(other.gameObject);
        }
        else
        {
            // First entry sets up damage timer
            if (!nextDamageTime.ContainsKey(other.gameObject))
                nextDamageTime[other.gameObject] = 0f;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (damageOnce) return; // Don't do this for one-time hitboxes

        var health = other.GetComponent<NPA_Health_Components.Health>();
        if (health == null) return;

        if (!nextDamageTime.ContainsKey(other.gameObject))
            nextDamageTime[other.gameObject] = 0f;

        if (Time.time >= nextDamageTime[other.gameObject])
        {
            health.TakeDamage(damage);
            nextDamageTime[other.gameObject] = Time.time + repeatDamageInterval;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Clean up timers when player leaves the area
        if (!damageOnce && nextDamageTime.ContainsKey(other.gameObject))
            nextDamageTime.Remove(other.gameObject);
    }
}
