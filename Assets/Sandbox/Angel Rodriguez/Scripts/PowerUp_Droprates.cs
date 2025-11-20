using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PowerUp_Droprates : MonoBehaviour
{
    List<KeyValuePair<GameObject, float>> powerUpDropRates = new List<KeyValuePair<GameObject, float>>();

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

    private EnemyHP enemyHP;
    private NPA_Health_Components.Health HealthComponent;
    
   void Start()
    {
        powerUpDropRates.Add(new KeyValuePair<GameObject, float>(FullhealPowerUpPrefab, FullHealthDropRate));
        powerUpDropRates.Add(new KeyValuePair<GameObject, float>(HalfHealPowerUpPrefab, HalfHealDropRate));
        powerUpDropRates.Add(new KeyValuePair<GameObject, float>(TenPercentHealPowerUpPrefab, TenPercentHealDropRate));
        powerUpDropRates.Add(new KeyValuePair<GameObject, float>(NoPowerUpPrefab, DropNothingRate));
    }

    public void DropPowerUp()
    {
        float randomValue = Random.Range(0f, 1f);
        float cumulativeProbability = 0f;

        foreach (var powerUp in powerUpDropRates)
        {
            cumulativeProbability += powerUp.Value;
            if (randomValue <= cumulativeProbability)
            {
                Instantiate(powerUp.Key, this.transform.position, Quaternion.identity);
                break;
            }
        }
    }
}
