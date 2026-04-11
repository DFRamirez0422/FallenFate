using UnityEngine;
using UnityEngine.UI;

public class Activate_Generators : CollidableObject
{
    [Header("Sprites")]
    [SerializeField] private Sprite Generator_Off;
    [SerializeField] private Sprite Generator_On;

    [Header("Generator Activation Settings")]
    public bool Activate_Generator = false;
    private bool _hasActivated = false;
    [SerializeField] private GameObject ActivateGeneratorPrompt;
    private GameObject _SpawnedPrompt;
    [SerializeField] private AudioSource GeneratorActivateSound;

    [Header("Objects To Activate")]
    [SerializeField] private GameObject[] objectsToActivate;

    [Header("Objects To Deactivate")]
    [SerializeField] private GameObject[] objectsToDeactivate;

    [SerializeField] private Item_Data item_Data;
    private PickUp_Manager _pickUp_Manager;

    protected override void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Generator_Off;
        _pickUp_Manager = GameObject.FindGameObjectWithTag("PickUp_Manager").GetComponent<PickUp_Manager>();
        ActivateGeneratorPrompt = Resources.Load<GameObject>("Prefabs/UI_Prefabs/ActionDescription");
        base.Start();
    }

    protected override void OnCollide(GameObject other)
    {
        if (_hasActivated) return;

        if (Input.GetButtonDown("Interact"))
        {
            ActivateGenerator();
        }
    }

    private void ActivateGenerator()
    {
        Activate_Generator = true;
        _hasActivated = true;

        GetComponent<SpriteRenderer>().sprite = Generator_On;

        if (_pickUp_Manager != null && item_Data != null)
        {
            _pickUp_Manager.items.Add(item_Data);
        }

        if (GeneratorActivateSound != null)
        {
            GeneratorActivateSound.Play();
        }

        ActivateAssignedObjects();
        DeactivateAssignedObjects();
    }

    private void ActivateAssignedObjects()
    {
        if (objectsToActivate == null || objectsToActivate.Length == 0)
            return;

        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            if (objectsToActivate[i] != null)
            {
                objectsToActivate[i].SetActive(true);
            }
        }
    }

    private void DeactivateAssignedObjects()
    {
        if (objectsToDeactivate == null || objectsToDeactivate.Length == 0)
            return;

        for (int i = 0; i < objectsToDeactivate.Length; i++)
        {
            if (objectsToDeactivate[i] != null)
            {
                objectsToDeactivate[i].SetActive(false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D hitCollider = collision.collider;
        if (hitCollider.CompareTag("Hitboxs"))
        {
            _SpawnedPrompt = Instantiate(ActivateGeneratorPrompt);

            if (!Activate_Generator)
            {
                _SpawnedPrompt.GetComponentsInChildren<Text>()[0].text = "Activate Generator";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[1].text = "[x]";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[2].text = "";
                _SpawnedPrompt.SetActive(true);
            }
            else
            {
                _SpawnedPrompt.GetComponentsInChildren<Text>()[0].text = "Generator Activated";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[1].text = "";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[2].text = "";
                _SpawnedPrompt.SetActive(true);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Collider2D hitCollider = collision.collider;
        if (hitCollider.CompareTag("Hitboxs"))
        {
            if (Activate_Generator && _SpawnedPrompt != null)
            {
                _SpawnedPrompt.GetComponentsInChildren<Text>()[0].text = "";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[1].text = "";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[2].text = "Generator Activated";
                _SpawnedPrompt.SetActive(true);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Collider2D hitCollider = collision.collider;
        if (hitCollider.CompareTag("Hitboxs"))
        {
            if (_SpawnedPrompt != null)
            {
                _SpawnedPrompt.SetActive(false);
                _SpawnedPrompt.GetComponentsInChildren<Text>()[0].text = "";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[1].text = "";
                _SpawnedPrompt.GetComponentsInChildren<Text>()[2].text = "";
                Destroy(_SpawnedPrompt);
            }
        }
    }
}