using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ElenaAI : MonoBehaviour
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
    }

}
