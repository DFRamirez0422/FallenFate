using System.Threading;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMash : MonoBehaviour
{
    public float mashDelay = 0.5f;
    public GameObject MashCanvas;
    public TextMeshProUGUI text;
    public TextMeshProUGUI text2;

    [SerializeField]
    private float mash, timer, points;
    private bool pressed;
    

    [HideInInspector]
    public bool started, stunned, knocked;

    //animator
    private Animator animator;

    //Private Called Scripts
    private PlayerHealth health;
    private PlayerMovement playerMovement;
    private SpriteRenderer PlayerSprite;
    private CinemachineImpulseSource playerImpulseSource;
    private CinemachineImpulseSource GrabberImpulseSource;
    private EnemyHitScript impactScript;

    //Public Called Scripts
    public GameObject Hitbox;
    public GrabberMovement grabberMovement;

    [Header("Audio")]
    public AudioSource DeathScream;
    public AudioSource MovementAudio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Hitbox.SetActive(false);
        animator = GetComponent<Animator>();
        MashCanvas.SetActive(false);
        mash = 1f;
        text2.enabled = false;
        PlayerSprite = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();
        GrabberImpulseSource = GetComponent<CinemachineImpulseSource>();
        impactScript = GetComponent<EnemyHitScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            PlayerSprite.enabled = false;
            playerMovement.Disable();
            playerMovement.m_Rigidbody.linearVelocity = Vector2.zero;

            timer += Time.deltaTime;

            MashCanvas.SetActive(true);
            mash -= Time.deltaTime;

            text.enabled = true;
            text.text = "Mash Z";

            animator.SetBool("Grabbing", true);

            if (Input.GetButtonDown("Attack") && !pressed)
            {
                pressed = true;
                points = points + 0.5f;
                impactScript.ImpactEffect();
                GrabberImpulseSource.GenerateImpulse();
                mash = mashDelay;
            }
            else if (Input.GetButtonUp("Attack"))
            {
                pressed = false;
            }

            if (health != null)
            {
                //Damage the player 
                if (mash <= 0)
                {
                    text2.enabled = true;
                    text2.text = "Damaged";
                    health.ChangeHealth(-1);
                    mash = 2.5f;
                    timer = 0;
                    Invoke(nameof(ToggleDamageText), 0.5f);
                }
            }
            else
            {
                Debug.LogWarning("Health is null");
            }

            //Stun the enemy
            if (points >= 5)
            {
                started = false;
                text.text = "Stunned";
                stunned = true;
                mash = 2.5f;
                Invoke(nameof(Unstun), 2);
                animator.SetBool("Attacking", false);
                animator.SetBool("Grabbing", false);
                grabberMovement.StoppedGrabbing();
                Invoke(nameof(EnablePlayer), 0.2f);
            }
        }
        else if (!stunned)
        { text.enabled = false; }

        if (stunned)
        {
            grabberMovement.rb.linearVelocity = Vector2.zero;
            animator.SetBool("Stunned", true);
            points = 0;
        }

        if (animator.GetBool("Died"))
        {
            EnablePlayer();
        }
    }

    private void Unstun()
    {
        if (stunned)
        {
            stunned = false;
            animator.SetBool("Attacking", false);
            animator.SetBool("Stunned", false);
        }

        if (knocked)
        {
            knocked = false;
        }
    }
    private void ToggleDamageText()
    {
        text2.enabled = false;
    }

    private void EnablePlayer()
    {
        if (playerMovement != null) {
        playerMovement.Enable();
        PlayerSprite.enabled = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject);
        if (collision.gameObject.tag == "Player")
        {
            playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            health = collision.gameObject.GetComponent<PlayerHealth>();
            timer = 0;
            animator.SetBool("Attacking", true); //The started variable gets activated when the grabber hitbox hits you in the attack animation.
        }
    }

    private void TurnOnHitbox()
    {
        Hitbox.SetActive(true);
        Invoke(nameof(TurnOffHitbox), 1);
    }

    private void TurnOffHitbox()
    {
        if (started)
        {
            Hitbox.SetActive(false);
        }
        else
        {
            animator.SetBool("Attacking", false);
            Hitbox.SetActive(false);
        }
    }

    public void NathansKnockbackClone()
    {
        knocked = true;

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        Rigidbody2D grabberRB = GetComponent<Rigidbody2D>();

        Vector2 direction = (transform.position - playerTransform.position).normalized;
        
        // Apply knockback velocity
        grabberRB.AddForce(direction * 400, ForceMode2D.Impulse);

        Debug.Log("Knocked");

        Invoke(nameof(Unstun), 0.5f);
    }

    private void PlayDeath()
    {
        DeathScream.Play();
    }

    private void PlayMovement()
    {
        MovementAudio.Play();
    }
}
