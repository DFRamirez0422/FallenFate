using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ElenaAI_JoseE : MonoBehaviour
{
    private Transform player;
    private Transform Elena;
    private NavMeshAgent agent;

    public int followDistance;
    public bool CombatToggle = false;
    public GameObject[] objects;
    
    [Header("Throw_PowerUp")]
    public int PowerUpHold = 0;
    public GameObject PowerUp;
    public List<GameObject> PowerUpsInGame = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // vvvvv Added by Jose E. from original file. vvvvv //

    [Header("QTE Events")]
    [SerializeField] private QuickTimeEvent_JoseE m_QTEvent;
    private bool m_IsQueryingQTEvent = false;

    // ^^^^^ Added by Jose E. from original file. ^^^^^ //

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;;


        Elena = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        
    }
    void Start()
    {
        objects = GameObject.FindGameObjectsWithTag("Cover");
    }

    // Update is called once per frame
    void Update()
    {
        if (PowerUpsInGame.Count != 0 && PowerUpHold == 0 && CombatToggle ||
            PowerUpsInGame.Count != 0 && PowerUpHold == 0 && !CombatToggle)
        {
            RetrivePowerUp();
        }

        else if (!CombatToggle && PowerUpHold == 1 || PowerUpsInGame.Count == 0 && !CombatToggle){ FollowPlayer(); }

        else if (CombatToggle && PowerUpHold == 1 || PowerUpsInGame.Count == 0 && CombatToggle) { TakeCover(); }

        UpdateQuickTimeEvent(); // ADDED BY: Jose E.
    }


    private void FollowPlayer()
    {
        float distanceToPlayer = Vector3.Distance(player.position, Elena.transform.position);

        if (player != null)
        {
            if (distanceToPlayer > followDistance)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                agent.SetDestination(Elena.position);
                return;
            }
        }
    }

    private void TakeCover()
    {
        GameObject closest = null;
        float minDistSq = float.MaxValue;
        Vector3 tPos = Elena.position;

        foreach (var obj in objects)
        {
            float distSq = (obj.transform.position - tPos).sqrMagnitude;
            if (distSq < minDistSq)
            {
                minDistSq = distSq;
                closest = obj;
            }
        }

        agent.SetDestination(closest.transform.position);
    }

   public void RetrivePowerUp()
   {
        GameObject  NearestPowerUp = null;
        float minDistSq = float.MaxValue;
        Vector3 ElenaPosition = Elena.position;

        foreach (var _PowerUp in PowerUpsInGame)
        {
            float distSq = (_PowerUp.transform.position - ElenaPosition).sqrMagnitude;
            if (distSq < minDistSq)
            {
                minDistSq = distSq;
                NearestPowerUp = _PowerUp;
            }
        }

        agent.SetDestination(NearestPowerUp.transform.position);
   }

    public void ThrowPowerUp()
    {
        GameObject PowerHealth = null;
        PowerHealth = Instantiate(PowerUp, transform.position, Quaternion.identity);
        PowerHealth.GetComponent<Rigidbody>().MovePosition(player.position);

        // vvvvv Added by Jose E. from original file. vvvvv //

        /// QuickTimeEvent has a start method; this is fine.
        /// Of all the public fields there are, there are none that are useful to query if the QTE
        /// was performed right; this is baffling.
        /// No return values on any of the public methods; what can we do now?
        /// Let's just pretend the quick time event was success and see what happens!
        /// 
        /// For now, I create a new copy of the script and modify it myself in the sandbox.
        m_QTEvent.StartQTE();
        m_IsQueryingQTEvent = true;


        // ^^^^^ Added by Jose E. from original file. ^^^^^ //
    }

    private void UpdateQuickTimeEvent()
    {
        if (!m_IsQueryingQTEvent) return;

        if (!m_QTEvent.IsStillRunning)
        {
            if (m_QTEvent.IsHitOnTime)
            {
                /// FIXME: something about this doesn't seem right, but I don't know where else this piece
                /// of code should go. I guess Elena is meant to directly add to the player inventory? I don't
                /// think I was told how this mechanic should work whatsoever. I don't know where the player
                /// inventory even is for that matter. The instructions below don't quite explain much, sad
                /// to say.
                /// 
                /// INSTRUCTIONS:
                /// 1. Player kils enemy
                /// 2. Enemy Drops Item
                /// 3. Elena Locates Item and walk to it
                /// 4. QTE Prompt appears this is you
                ///   A. Player succeds, item is catched
                ///   B. Player fails, no item is caught
                /// *make sure to add console debugs that tell us the item was successful or not in being caught.
                Debug.Log("I hit on time! Adding to the player inventory...");
            }
            else
            {
                Debug.Log("No, item dropped...");
            }

            m_IsQueryingQTEvent = false;
        }
    }

}
