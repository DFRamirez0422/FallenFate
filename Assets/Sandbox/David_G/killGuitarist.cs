using UnityEngine;

public class killGuitarist : MonoBehaviour
{
    [SerializeField] private GameObject guitarist;
    [SerializeField] private Animator guitaristAnimator;
    [SerializeField] private bool isDead = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        guitaristAnimator = guitarist.GetComponent<Animator>();
    }

    void Update()
    {
        checkIfGuitaristIsDead();
    }

    public void checkIfGuitaristIsDead()
    {
        if (isDead)
        {
            guitaristAnimator.SetTrigger("Dead");
        }
    }
}
