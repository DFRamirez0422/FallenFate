using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PowerUp_Droprates : MonoBehaviour
{
    public class Drops
    {
        public GameObject PowerUpPrefab {get; set; }
        public float DropRate {get; set; }
    }

    List<Drops> dropsList = new List<Drops>();

    [Header("Drop Rates (Total must equal 1.0)")]
    public float BloodyHeartDropRate;
    public float CrackedPickDropRate;
    public float GuitarStingDropRate;
    public float DropNothingRate;

    [Header("PowerUp Prefabs")]
    public GameObject BloodyHeartPower;
    public GameObject CrackedPickPower;
    public GameObject GuitarSting;
    public GameObject NoPowerUpPrefab;
    
   void Start()
    {
        dropsList.Add(new Drops { PowerUpPrefab =  BloodyHeartPower, DropRate = BloodyHeartDropRate });
        dropsList.Add(new Drops { PowerUpPrefab = CrackedPickPower, DropRate = CrackedPickDropRate });
        dropsList.Add(new Drops { PowerUpPrefab = GuitarSting, DropRate = GuitarStingDropRate });
        dropsList.Add(new Drops { PowerUpPrefab = NoPowerUpPrefab, DropRate = DropNothingRate });
    }

    public void DropPowerUp()
    {
        float randomValue = Random.Range(0f, 1f);
        float cumulativeProbability = 0f;
        
        foreach (var powerUp in dropsList)
        {
            cumulativeProbability += powerUp.DropRate;

            if (randomValue <= cumulativeProbability)
            {
                Vector3 SpawnPosition = this.transform.position + Vector3.up * 0.1f;
                Instantiate(powerUp.PowerUpPrefab, SpawnPosition, Quaternion.identity);
                break;
            }
        }
    }
}
