using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableObject : MonoBehaviour
{
    private Collider2D collidableCollider; // The collider of this object

    [SerializeField]
    private ContactFilter2D contactFilter; // Filter to specify which colliders to check against
    private List<Collider2D> results = new List<Collider2D>(1);

    protected virtual void Start()
    {
        collidableCollider = GetComponent<Collider2D>(); // Get the collider component
    }

    protected virtual void Update()
    {
         collidableCollider.Overlap(contactFilter, results); // Check for overlapping colliders
            foreach (var collider in results)
            {
                OnCollide(collider.gameObject);
            }
    }

   // Method to be overridden by the PickUpObjects class
    protected virtual void OnCollide(GameObject other)
    {
        Debug.Log("Collided with " + other.name);
    }
}
