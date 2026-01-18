using UnityEngine;

public class PickUp_Manager : MonoBehaviour
{
    [SerializeField] private GameObject Memento1;
    private bool Memento1_Collected = false;
    [SerializeField] private GameObject Memento2;
    private bool Memento2_Collected = false;

    [SerializeField] private GameObject Memento3;
    private bool Memento3_Collected = false;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Functions to activate mementos when picked up and
    // give the Gameobjects above the mementos the Prefab from Resources/Prefabs folder and set collected to true
    public void ActivateMemento1()
    {
        Memento1 = Resources.Load("Prefabs/Memento1") as GameObject;
        Memento1_Collected = true;
    }

    public void ActivateMemento2()
    {
        Memento2 = Resources.Load("Prefabs/Memento2") as GameObject;
        Memento2_Collected = true;
    }

    public void ActivateMemento3()
    {
        Memento3 = Resources.Load("Prefabs/Memento3") as GameObject;
        Memento3_Collected = true;
    }

}
