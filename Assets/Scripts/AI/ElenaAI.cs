using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ElenaAI : MonoBehaviour
{
    private Transform player;
    private Transform Elena;
    private NavMeshAgent agent;

    public int followDistance;
    public bool CombatToggle = false;
    public GameObject[] objects;
    
    [Header("Throw_PowerUp + UI Settings")]
    public int PowerUpHold = 0;
    public GameObject PowerUp;
    public Transform SpawnedPowerUpposition;
    public List<GameObject> PowerUpsInGame = new List<GameObject>();
  


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        // Initialize UI for throw
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

        Vector3 eulerAngles = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0,0,0);

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

   //---- Change made by Angel.Rodriguez ----//
   // Retrieve the nearest power-up from the ground
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

    // Throw the held power-up towards the player
    public void ThrowPowerUp()
    {
        NPA_PlayerPrefab.Scripts.PlayerController playerScript = player.GetComponent<NPA_PlayerPrefab.Scripts.PlayerController>();
        playerScript.IsCatchingPowerUp = true;

        GameObject PowerHealth = Instantiate(PowerUp, SpawnedPowerUpposition.position, Quaternion.identity);
        PowerUpPickups powerUp = PowerHealth.GetComponent<PowerUpPickups>();
        powerUp.isThrown = true;
        Rigidbody rb = PowerHealth.GetComponent<Rigidbody>();

        float animation = 0f;
        animation += Time.deltaTime;
        animation = animation % 2.5f;

     //   rb.AddForce((player.position - SpawnedPowerUp.position).normalized * 10f, ForceMode.VelocityChange);
          rb.linearVelocity = (player.position - SpawnedPowerUpposition.position).normalized * 10f;
    }  

    
}
