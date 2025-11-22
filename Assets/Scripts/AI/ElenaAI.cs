using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    public List<GameObject> PowerUpsInGame = new List<GameObject>();
    public GameObject PowerUpIcon;
    public GameObject BackgroundIcon;
    public Text BButtonText;
    [SerializeField] private Sprite NoPowerUpIcon;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        // Initialize UI for throw
        BButtonText.color = Color.midnightBlue;
        BackgroundIcon.SetActive(false);

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
        GameObject PowerHealth = null;
        PowerUpIcon.GetComponent<Image>().sprite = NoPowerUpIcon;
        BButtonText.color = Color.midnightBlue;
        Vector3 SpawnPosition = transform.position + transform.forward * 2f + Vector3.up * 1f;
        PowerHealth = Instantiate(PowerUp, SpawnPosition, Quaternion.identity);
        Vector3 DirectionToPlayer = (player.position - transform.position).normalized;
        DirectionToPlayer.y = 0f;
        Rigidbody rb = PowerHealth.GetComponent<Rigidbody>();
        rb.linearVelocity = DirectionToPlayer * 5f + Vector3.up * 3f;
    }

}
