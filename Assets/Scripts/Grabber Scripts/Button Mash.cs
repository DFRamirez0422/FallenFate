using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMash : MonoBehaviour
{
    public float mashDelay = 0.5f;
    public GameObject MashCanvas;
    public TextMeshProUGUI text;
    public TextMeshProUGUI text2;

    [SerializeField]
    private float mash, timer;
    private bool pressed;
    [HideInInspector]
    public bool started, stunned;

    //Called Scripts
    private SplayerHealth health;
    private PlayerMovement playerMovement;
    //private Enemy_Movement movement; -Not used right now but will probably use later.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MashCanvas.SetActive(false);
        mash = mashDelay;
        text2.enabled = false;
        //movement = GetComponent<Enemy_Movement>(); -Not used right now but will probably use later.
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            playerMovement.enabled = false;
            playerMovement.rb.velocity = Vector2.zero;

            timer += Time.deltaTime;

            MashCanvas.SetActive(true);
            mash -= Time.deltaTime;

            text.enabled = true;
            text.text = "Mash Z";
            

            if (Input.GetButtonDown("Attack") && !pressed)
            {
                pressed = true;
                mash = mashDelay;
            }
            else if (Input.GetButtonUp("Attack"))
            {
                pressed = false;
            }

            if (health != null)
            {
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

            if (timer >= 3)
            {
                started = false;
                text.text = "Stunned";
                stunned = true;
                mash = 2.5f;
                Invoke(nameof(Unstun), 2);
            }            
        }
        else if (!stunned)
        {text.enabled = false;}


    }

    private void Unstun()
    {
        stunned = false;
    }
    private void ToggleDamageText()
    {
        text2.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
        health = collision.gameObject.GetComponent<SplayerHealth>();
        timer = 0;
        started = true;
    }
}
