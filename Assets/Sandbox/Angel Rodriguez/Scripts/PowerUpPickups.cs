using System.Collections.Generic;
using System.Data.Common;
using NPA_Health_Components;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PowerUpPickups : MonoBehaviour
{

     public enum PowerUpType { Heal, DamageBoost, ShieldReflect }

     [SerializeField] private GameObject BloodyHeart;
     [SerializeField] private GameObject CrackedPick;
     [SerializeField] private GameObject GuitarString;

     private string BloodyHeartName => BloodyHeart.name;
     private string CrackedPickName => CrackedPick.name;
     private string GuitarStringName => GuitarString.name;

    [Header("Item Settings")]
    public PowerUpType type;
    private bool Colliding;
    [HideInInspector] public bool isThrown;
    
    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Elena"))
        {
            ElenaAI _ElenaAi = other.gameObject.GetComponent<ElenaAI>();
            if (_ElenaAi != null && !isThrown)
            {
                if(this.gameObject.name == BloodyHeartName)
                {
                    _ElenaAi.PowerUp = Resources.Load("BloodyHeart") as GameObject;;
                }
                if(this.gameObject.name == CrackedPickName)
                {
                    _ElenaAi.PowerUp = Resources.Load(CrackedPickName) as GameObject;;
                }
                if(this.gameObject.name == GuitarStringName)
                {
                    _ElenaAi.PowerUp = Resources.Load(GuitarStringName) as GameObject;;
                }
              _ElenaAi.PowerUpHold = 1;
              var copy = this.gameObject;
              Destroy(copy);
             }

        }

        if(other.gameObject.CompareTag("Player") && isThrown)
        {
           PowerUpeffects _powerUpeffects = other.gameObject.GetComponent<PowerUpeffects>();
           NPA_PlayerPrefab.Scripts.PlayerController playerScript = other.gameObject.GetComponent<NPA_PlayerPrefab.Scripts.PlayerController>();
           if(_powerUpeffects != null)
            {
                if(this.gameObject.name == BloodyHeartName)
                {
                    _powerUpeffects.FullHeal();
                    playerScript.IsCatchingPowerUp = false;
                }
                if(this.gameObject.name == CrackedPickName)
                {
                    _powerUpeffects.Heal(0.25f);
                    playerScript.IsCatchingPowerUp = false;
                }
                if(this.gameObject.name == GuitarStringName)
                {
                    _powerUpeffects.Heal(0.10f);
                    playerScript.IsCatchingPowerUp = false;
                }
              var copy = this.gameObject;
              Destroy(copy);
            }
        }

    }

    



}

