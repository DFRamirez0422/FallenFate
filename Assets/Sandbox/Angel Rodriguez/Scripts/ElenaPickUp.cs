using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ElenaPickUp : MonoBehaviour
{
    // Class to hold power-up data
    public class ElenaFindPowerUp
    {
        public GameObject PowerUp { get; set; }
        public string PowerUpName { get; set; }
        public Sprite PowerUpSprite { get; set; }
    }

    [Header("Elena_PickUps")]
    [SerializeField] private GameObject E_FullHealthPower;
    [SerializeField] private GameObject E_HalfHealPower;
    [SerializeField] private GameObject E_TenPercentHealthPower;
    public Sprite E_FullHealthIcon;
    public Sprite E_HalfHealIcon;
    public Sprite E_TenPercentHealthIcon;


    [Header("Damian_Heal")]
    [SerializeField] private GameObject D_FullHealthPower;
    [SerializeField] private GameObject D_HalfHealPower;
    [SerializeField] private GameObject D_TenPercentHealthPower;

        // Lists to hold power-up data
        List<ElenaFindPowerUp> ElenaPowerUpList = new List<ElenaFindPowerUp>();
        List<ElenaFindPowerUp> DamianPowerUpList = new List<ElenaFindPowerUp>();

        // Initialize power-up lists
    void Awake()
    {
        ElenaPowerUpList.Add(new ElenaFindPowerUp { PowerUp = E_FullHealthPower, PowerUpName = E_FullHealthPower.name, PowerUpSprite = E_FullHealthIcon });
        ElenaPowerUpList.Add(new ElenaFindPowerUp { PowerUp = E_HalfHealPower, PowerUpName = E_HalfHealPower.name, PowerUpSprite = E_HalfHealIcon });
        ElenaPowerUpList.Add(new ElenaFindPowerUp { PowerUp = E_TenPercentHealthPower, PowerUpName = E_TenPercentHealthPower.name, PowerUpSprite = E_TenPercentHealthIcon });

        DamianPowerUpList.Add(new ElenaFindPowerUp { PowerUp = D_FullHealthPower, PowerUpName = D_FullHealthPower.name});
        DamianPowerUpList.Add(new ElenaFindPowerUp { PowerUp = D_HalfHealPower, PowerUpName = D_HalfHealPower.name});
        DamianPowerUpList.Add(new ElenaFindPowerUp { PowerUp = D_TenPercentHealthPower, PowerUpName = D_TenPercentHealthPower.name});
    }

    // Detect collision with Elena to give power-up
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Elena"))
        {
            ElenaAI _ElenaAi = other.gameObject.GetComponent<ElenaAI>();
            Image PowerUpIcon = _ElenaAi.PowerUpIcon.GetComponent<Image>();
            if (_ElenaAi != null )
            {
                if (_ElenaAi.PowerUpHold == 0)
                {
                    // Lock player movement when power-up is grabbed
                    GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                    if (playerObj != null)
                    {
                        NPA_PlayerPrefab.Scripts.PlayerController playerController = playerObj.GetComponent<NPA_PlayerPrefab.Scripts.PlayerController>();
                        if (playerController != null)
                        {
                            playerController.LockMovementForThrow(0.3f); // Brief lock on grab
                        }
                    }
                    
                    if(this.gameObject.name == ElenaPowerUpList[0].PowerUpName)
                    {
                        PowerUpIcon.sprite = ElenaPowerUpList[0].PowerUpSprite;
                        _ElenaAi.PowerUp = DamianPowerUpList[0].PowerUp;
                        _ElenaAi.BButtonText.color = Color.darkRed;
                        _ElenaAi.BackgroundIcon.SetActive(true);
                        _ElenaAi.PowerUpHold = 1;
                    }
                    else if (this.gameObject.name == ElenaPowerUpList[1].PowerUpName)
                    {
                        PowerUpIcon.sprite = ElenaPowerUpList[1].PowerUpSprite;
                        _ElenaAi.PowerUp = DamianPowerUpList[1].PowerUp;
                        _ElenaAi.BButtonText.color = Color.darkRed;
                        _ElenaAi.BackgroundIcon.SetActive(true);
                        _ElenaAi.PowerUpHold = 1;

                    }
                    else if (this.gameObject.name == ElenaPowerUpList[2].PowerUpName)
                    {
                        PowerUpIcon.sprite = ElenaPowerUpList[2].PowerUpSprite;
                        _ElenaAi.PowerUp = DamianPowerUpList[2].PowerUp;
                        _ElenaAi.BButtonText.color = Color.darkRed;
                        _ElenaAi.BackgroundIcon.SetActive(true);
                        _ElenaAi.PowerUpHold = 1;
                    }
                    var copyH = this.gameObject;
                    Destroy(copyH);
                }
            }
            else { }
        }
        else { }
    }
}
