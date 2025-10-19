using NUnit.Framework;
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
    [SerializeField] private GameObject HPowerUP;
    [SerializeField] private GameObject HealthPower;
    public int HealthPackHold = 0;

    [SerializeField]private GameObject[] HealthPowerInGame;
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
        HealthPowerInGame = GameObject.FindGameObjectsWithTag("HealthPickup");

        if (!CombatToggle){ FollowPlayer(); }

        if(CombatToggle || !CombatToggle && HealthPackHold == 0 && HealthPowerInGame != null) { RetrieveHealthPack(); }

        else if (CombatToggle && HealthPackHold == 1 || CombatToggle && HealthPowerInGame == null || 
            HealthPowerInGame != null && HealthPackHold == 1 && CombatToggle) { TakeCover(); }


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

        if(!CombatToggle && HealthPackHold == 1)
        {
            FollowPlayer();
        }

        else if (HealthPackHold == 0 && CombatToggle || !CombatToggle){
            RetrieveHealthPack();
        }

    }

    private void RetrieveHealthPack()
    {
        GameObject closestHealthPack;
        float minDistSq = float.MaxValue;
        Vector3 tPos = Elena.position;

        foreach (var objH in HealthPowerInGame)
        {
            float distSq = (objH.transform.position - tPos).sqrMagnitude;
            if (distSq < minDistSq)
            {
                minDistSq = distSq;
                closestHealthPack = objH;
                if (HealthPowerInGame != null && HealthPackHold == 0)
                {
                    agent.SetDestination(closestHealthPack.transform.position);
                }
            }
        }

        if (HealthPackHold == 1 && CombatToggle)
        {
            TakeCover();
        }
        else if (HealthPackHold == 1 && !CombatToggle)
        {
            FollowPlayer();
        }
    }



    public void ThrowHealthPowerUP()
    {
        
        int speed = 2;
        Vector3 SpawnHpower = new Vector3(Elena.transform.position.x, Elena.transform.position.y + 0.5f, Elena.transform.position.z + 0.5f);        
        HPowerUP = Instantiate(HealthPower, SpawnHpower, Quaternion.identity);

        Vector3 Targetdirection = (player.position - SpawnHpower).normalized;
        HPowerUP.GetComponent<Rigidbody>().MovePosition(player.position + Targetdirection * speed * Time.deltaTime);
    }
}
