using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class E_PowerUpFind : MonoBehaviour
{
    ElenaAI _ElenaAI;
    private GameObject game;

    private void Start()
    {
        _ElenaAI = GameObject.FindGameObjectWithTag("Elena").GetComponent<ElenaAI>();
    }

    private void Update()
    {
        _ElenaAI.PowerUpsInGame.RemoveAll(item => item == null);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PowerUp"))
        {
            _ElenaAI.PowerUpsInGame.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("PowerUp"))
        {
             _ElenaAI.PowerUpsInGame.Remove(other.gameObject);
            Debug.Log($"Removed {other.gameObject.name} from the list.");
        }
    }
}
