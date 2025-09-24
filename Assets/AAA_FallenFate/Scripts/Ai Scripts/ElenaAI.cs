using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;

public class ElenaAI : MonoBehaviour
{
    
    private Transform player;
    private Transform self;
    private NavMeshAgent agent;

    public int followDistance;
    public bool CombatToggle = false;
    public GameObject[] objects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        self = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        
    }
    void Start()
    {
        objects = GameObject.FindGameObjectsWithTag("Cover");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (CombatToggle == false)
            {
                CombatToggle = true;
            }
            else if (CombatToggle == true)
            {
                CombatToggle = false;
            }

        }
        if (CombatToggle == false)
        {
            FollowPlayer();
            
        }

        if (CombatToggle == true)
        {
            TakeCover();
            
        }       
    }

    private void FollowPlayer()
    {
        float distanceToPlayer = Vector3.Distance(player.position, self.transform.position);

        if (player != null)
        {
            if (distanceToPlayer > followDistance)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                agent.SetDestination(self.position);
                return;
            }
        }
    }

    private void TakeCover()
    {
        GameObject closest = null;
        float minDistSq = float.MaxValue;
        Vector3 tPos = self.position;

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
}
