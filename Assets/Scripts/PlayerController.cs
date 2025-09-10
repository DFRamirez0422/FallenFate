// Assets/Scripts/PlayerController.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Rigidbody for applying physics-based movement - BR
    public Rigidbody theRB;
    public float moveSpeed, jumpForce;

    private Vector2 moveInput;

    // Ground check setup - BR
    public LayerMask whatIsGround;
    public Transform groundPoint;
    private bool isGrounded;

    // Animation components - BR
    public Animator anim;
    public SpriteRenderer theSR;
    private bool movingBackwards;
    public Animator flipAnim;

    void Start()
    {
        // Initialization if needed - BR
    }

    void Update()
    {
        // Get player movement input from keyboard/controller - BR
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        moveInput.Normalize(); // Ensures consistent speed diagonally - BR

        // Apply movement to Rigidbody using linear velocity - BR
        theRB.linearVelocity = new Vector3(moveInput.x * moveSpeed, theRB.linearVelocity.y, moveInput.y * moveSpeed);

        // Set animation float based on current movement speed - BR
        anim.SetFloat("moveSpeed", theRB.linearVelocity.magnitude);

        // Cast a short ray down to detect if player is grounded - BR
        RaycastHit hit;
        if (Physics.Raycast(groundPoint.position, Vector3.down, out hit, .3f, whatIsGround))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        // Allow jumping if player is grounded - BR
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            theRB.linearVelocity += new Vector3(0f, jumpForce, 0f);
        }

        // Update animator with grounded state - BR
        anim.SetBool("onGround", isGrounded);

        // Flip sprite and trigger animation when changing horizontal direction - BR
        if (!theSR.flipX && moveInput.x < 0)
        {
            theSR.flipX = true;
            flipAnim.SetTrigger("Flip");
        }
        else if (theSR.flipX && moveInput.x > 0)
        {
            theSR.flipX = false;
            flipAnim.SetTrigger("Flip");
        }

        // Detect backward movement (moving up in world space) and flip accordingly - BR
        if (!movingBackwards && moveInput.y > 0)
        {
            movingBackwards = true;
            flipAnim.SetTrigger("Flip");
        }
        else if (movingBackwards && moveInput.y < 0)
        {
            movingBackwards = false;
            flipAnim.SetTrigger("Flip");
        }

        // Update animator with backward movement state - BR
        anim.SetBool("movingBackwards", movingBackwards);
    }
}
