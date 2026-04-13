using UnityEngine;

public class WardenSpawn : MonoBehaviour
{
    public GameObject Warden;
    public Transform targetSpawnPoint;
    public static bool reset;
    public bool activeTP = true;
    private Animator animator;
    private WardenMovement movement;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Warden = GameObject.FindGameObjectWithTag("Warden");
        targetSpawnPoint = GetComponent<Transform>();
        animator = GameObject.FindWithTag("Warden").GetComponent<Animator>();
        movement = GameObject.FindWithTag("Warden").GetComponent<WardenMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (reset == true)
        {
            activeTP = true;
            Debug.Log("Spawners reset");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && activeTP == true)
        {
            reset = true;
            
            Invoke(nameof(disabletp), 0.5f);
            Invoke(nameof(Despawn), 0.5f);
        }
    }

    void disabletp()
    {
        reset = false;
        activeTP = false;
    }

    public void Despawn()
    {
        movement.stunned = true;
        animator.SetBool("Despawn", true);
        Invoke(nameof(Respawn), 1);
    }

    public void Respawn()
    {
        movement.stunned = true;
        animator.SetBool("Despawn", false);
        Warden.transform.position = targetSpawnPoint.position;
        animator.SetBool("Respawn", true);
        animator.SetBool("Attacking", false);
    }
}
