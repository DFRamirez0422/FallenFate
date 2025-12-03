using System.Collections.Generic;
using NPA_Health_Components;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using System;

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

    [Header("parabolic Settings")]
    protected float ani;
    public float height = 5f;
    private Transform startPos;
    private Transform targetPos;
    private ElenaAI Elena;

    void Awake()
    {
        Elena = GameObject.FindGameObjectWithTag("Elena").GetComponent<ElenaAI>();
        startPos = Elena.SpawnedPowerUpposition;
        targetPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>(); 
    }

    void Update()
    {
        Elena = GameObject.FindGameObjectWithTag("Elena").GetComponent<ElenaAI>();
        startPos = Elena.SpawnedPowerUpposition;
        targetPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>(); 

        if (isThrown)
        {
            ani += Time.deltaTime;
            ani = ani % 5f;

            transform.position = ParabolicVelocity(startPos.position, targetPos.position, height, ani / 1f);
        }
    }

     private

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Elena"))
        {
            ElenaAI _ElenaAi = other.gameObject.GetComponent<ElenaAI>();
            if (_ElenaAi != null && !isThrown && _ElenaAi.PowerUpHold == 0)
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

        if(other.gameObject.CompareTag("Player") && isThrown 
            || other.gameObject.CompareTag("Player") && !isThrown)
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

    
        private Vector3 ParabolicVelocity(Vector3 source, Vector3 target, float height, float gravity)
    {
        Func<float, float> f = x =>  -4 * height * x * x + 4 * height * x;
        var midd = Vector3.Lerp(source, target, gravity);
        return new Vector3(midd.x, f(gravity) + Mathf.Lerp(source.y, target.y, gravity), midd.z);
    }


}

