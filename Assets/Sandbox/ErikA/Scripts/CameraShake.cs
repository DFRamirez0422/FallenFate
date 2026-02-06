using UnityEngine;

public class CameraShake : MonoBehaviour
{

    [SerializeField] private float shakeAmount = 0.2f;
    private Vector3 initialPos;

    public void Awake()
    {
         initialPos = transform.position;
    }

    // Update is called once per frame
    public void Update()
    {
      
    }

    public void PlayShake()
    {
        transform.position = initialPos + Random.insideUnitSphere * shakeAmount;
        Debug.Log("Shake");
    }
}
