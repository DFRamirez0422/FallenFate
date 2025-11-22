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
    public float FullHealthDropRate;
    public float HalfHealDropRate;
    public float TenPercentHealDropRate;
    public float DropNothingRate;

    [Header("PowerUp Prefabs")]
    public GameObject FullhealPowerUpPrefab;
    public GameObject HalfHealPowerUpPrefab;
    public GameObject TenPercentHealPowerUpPrefab;
    public GameObject NoPowerUpPrefab;
    
   void Start()
    {
        dropsList.Add(new Drops { PowerUpPrefab = FullhealPowerUpPrefab, DropRate = FullHealthDropRate });
        dropsList.Add(new Drops { PowerUpPrefab = HalfHealPowerUpPrefab, DropRate = HalfHealDropRate });
        dropsList.Add(new Drops { PowerUpPrefab = TenPercentHealPowerUpPrefab, DropRate = TenPercentHealDropRate });
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
