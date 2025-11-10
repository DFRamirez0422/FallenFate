using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class E_PowerUpFind : MonoBehaviour
{
    ElenaAI _ElenaAI;
    private List<GameObject> PowerUps = new List<GameObject>();
    private GameObject game;

    private void Start()
    {
        _ElenaAI = GameObject.FindGameObjectWithTag("Elena").GetComponent<ElenaAI>();
    }

    private void Update()
    {
        PowerUps.RemoveAll(item => item == null);
        _ElenaAI.PowerUpsInGame = PowerUps;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("E_PowerUp"))
        {
            PowerUps.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("E_PowerUp"))
        {
             PowerUps.Remove(other.gameObject);
            Debug.Log($"Removed {other.gameObject.name} from the list.");
        }
    }
}
