using UnityEngine;

public class ElenaPickUp : MonoBehaviour
{
    [Header("Elena_PickUps")]
    [SerializeField] private GameObject E_FullHealthPower;
    [SerializeField] private GameObject E_HalfHealPower;
    [SerializeField] private GameObject E_TenPercentHealthPower;

    [Header("Damian_Heal")]
    [SerializeField] private GameObject D_FullHealthPower;
    [SerializeField] private GameObject D_HalfHealPower;
    [SerializeField] private GameObject D_TenPercentHealthPower;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Elena"))
        {
            ElenaAI _ElenaAi = other.gameObject.GetComponent<ElenaAI>();
            ElenaAI _ElenaPickUp = _ElenaAi;

            if (_ElenaPickUp != null )
            {
                if (_ElenaPickUp.PowerUpHold == 0)
                {
                    if(this.gameObject.name == E_FullHealthPower.name)
                    {
                        _ElenaPickUp.PowerUp = D_FullHealthPower;
                        _ElenaAi.PowerUpHold = 1;
                    }
                    else if (this.gameObject.name == E_HalfHealPower.name)
                    {
                        _ElenaPickUp.PowerUp = D_HalfHealPower;
                        _ElenaAi.PowerUpHold = 1;
                    }
                    else if (this.gameObject.name == E_TenPercentHealthPower.name)
                    {
                        _ElenaPickUp.PowerUp = D_TenPercentHealthPower;
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
