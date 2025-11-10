using UnityEngine;

public class BossMachineGunRiff : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform MGRPointA;
    [SerializeField] private Transform MGRPointB;
    [SerializeField] private Transform player;

    [Header("Machine Gun Riff Settings")]
    [SerializeField] private float fireRate = 0.08f;
    [SerializeField] private float duration = 4f;
    [SerializeField] private float projectileSpeed = 20f;     // controlled here
    [SerializeField] private float projectileLifetime = 5f;   // controlled here

    private Coroutine MGRCoroutine;

    public void StartMGR()
    {
        if (MGRCoroutine == null)
            MGRCoroutine = StartCoroutine(MGRRoutine());
    }

    public void StopMGR()
    {
        if (MGRCoroutine != null)
        {
            StopCoroutine(MGRCoroutine);
            MGRCoroutine = null;
        }
    }

    private System.Collections.IEnumerator MGRRoutine()
    {
        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            FireFrom(MGRPointA);
            FireFrom(MGRPointB);
            yield return new WaitForSeconds(fireRate);
        }
        MGRCoroutine = null;
    }

    private void FireFrom(Transform point)
    {
        if (!projectilePrefab || !point || !player) return;

        // Aim purely on XZ plane
        Vector3 toPlayer = player.position - point.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude < 0.0001f) toPlayer = point.forward;
        Vector3 dir = toPlayer.normalized;

        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        Vector3 spawnPos = point.position;

        var go = Instantiate(projectilePrefab, spawnPos, rot);

        // Configure projectile speed & lifetime from here
        var proj = go.GetComponent<MGRProjectile>();
        if (proj != null)
            proj.Arm(projectileSpeed, projectileLifetime);
    }
}
