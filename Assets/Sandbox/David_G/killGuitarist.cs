using UnityEngine;

public class killGuitarist : MonoBehaviour
{
    [SerializeField] private GameObject guitarist;
    [SerializeField] private Animator guitaristAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        guitaristAnimator = guitarist.GetComponent<Animator>();
    }

    void Update()
    {
        guitaristAnimator.SetBool("isDead", true);
    }
}
