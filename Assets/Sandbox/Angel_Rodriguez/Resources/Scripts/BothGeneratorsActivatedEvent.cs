using UnityEngine;
using UnityEngine.Events;

public class BothGeneratorsActivatedEvent : MonoBehaviour
{
    [Header("Generator References")]
    [SerializeField] private Activate_Generators generatorA;
    [SerializeField] private Activate_Generators generatorB;

    [Header("Event")]
    [SerializeField] private UnityEvent OnbothGeneratorsActivated;

    private bool hasInvoked;

    private void Update()
    {
        if (hasInvoked) return;
        if (generatorA == null || generatorB == null) return;

        if (generatorA.Activate_Generator && generatorB.Activate_Generator)
        {
            hasInvoked = true;
            OnbothGeneratorsActivated?.Invoke();
        }
    }
}
