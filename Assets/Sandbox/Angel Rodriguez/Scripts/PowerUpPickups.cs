using System.Collections.Generic;
using System.Data.Common;
using NPA_Health_Components;
using Unity.Mathematics;
using Unity.VisualScripting;
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

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Elena"))
        {
            ElenaAI _ElenaAi = other.gameObject.GetComponent<ElenaAI>();
            if (_ElenaAi != null )
            {
                if(gameObject.name == BloodyHeartName)
                {
                    _ElenaAi.PowerUp = BloodyHeart;
                }
                if(this.gameObject.name == CrackedPickName)
                {
                    _ElenaAi.PowerUp = CrackedPick;
                }
                if(this.gameObject.name == GuitarStringName)
                {
                    _ElenaAi.PowerUp = GuitarString;
                }
              _ElenaAi.PowerUpHold = 1;
              var copy = this.gameObject;
              Destroy(copy);
             }

        }

    }



}

