using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableObject : MonoBehaviour
{
    private Collider2D collidableCollider;

    [SerializeField]
    private ContactFilter2D contactFilter;
    private List<Collider2D> results = new List<Collider2D>();

    protected virtual void Start()
    {
        collidableCollider = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
         collidableCollider.Overlap(contactFilter, results);
            foreach (var collider in results)
            {
                OnCollide(collider.gameObject);
            }
    }

    protected virtual void OnCollide(GameObject other)
    {
        Debug.Log("Collided with " + other.name);
    }
}
