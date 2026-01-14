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
    private SplayerHealth health;
    private PlayerMovement movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MashCanvas.SetActive(false);
        mash = mashDelay;
        text2.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            movement.enabled = false;
            movement.rb.velocity = Vector2.zero;

            timer += Time.deltaTime;

            MashCanvas.SetActive(true);
            mash -= Time.deltaTime;

            text.enabled = true;
            text.text = "Mash Space";
            

            if (Input.GetKeyDown(KeyCode.Space) && !pressed)
            {
                pressed = true;
                mash = mashDelay;
            }
            else if (Input.GetKeyUp(KeyCode.Space))
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
                movement.enabled = true;
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
        movement = collision.gameObject.GetComponent<PlayerMovement>();
        health = collision.gameObject.GetComponent<SplayerHealth>();
        timer = 0;
        started = true;
    }
}
