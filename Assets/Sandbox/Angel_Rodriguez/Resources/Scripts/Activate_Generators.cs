using System.Collections;
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

    [Header("Despawn Flicker Settings")]
    [Tooltip("If true, objectsToDeactivate will visually flicker before being turned off.")]
    [SerializeField] private bool useFlickerDespawn = true;

    [Tooltip("How long the despawn flicker lasts.")]
    [SerializeField] private float despawnFlickerDuration = 0.7f;

    [Tooltip("Shortest delay between flicker toggles.")]
    [SerializeField] private float despawnFlickerMinInterval = 0.04f;

    [Tooltip("Longest delay between flicker toggles.")]
    [SerializeField] private float despawnFlickerMaxInterval = 0.1f;

    [Tooltip("If true, disables Collider2D components right away so the enemy stops interacting before it visually disappears.")]
    [SerializeField] private bool disableCollidersImmediately = true;

    [Tooltip("If true, disables Animator components right away so the enemy looks more 'frozen' while flickering out.")]
    [SerializeField] private bool disableAnimatorsImmediately = false;

    [Tooltip("If true, tries to disable all MonoBehaviours on the object except Transform-related internals. Use carefully.")]
    [SerializeField] private bool disableBehavioursImmediately = false;

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
            if (objectsToDeactivate[i] == null)
                continue;

            if (!useFlickerDespawn)
            {
                objectsToDeactivate[i].SetActive(false);
            }
            else
            {
                StartCoroutine(FlickerDeactivateObject(objectsToDeactivate[i]));
            }
        }
    }

    private IEnumerator FlickerDeactivateObject(GameObject target)
    {
        if (target == null)
            yield break;

        SpriteRenderer[] spriteRenderers = target.GetComponentsInChildren<SpriteRenderer>(true);
        Collider2D[] colliders = target.GetComponentsInChildren<Collider2D>(true);
        Animator[] animators = target.GetComponentsInChildren<Animator>(true);
        MonoBehaviour[] behaviours = target.GetComponentsInChildren<MonoBehaviour>(true);

        if (disableCollidersImmediately)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != null)
                    colliders[i].enabled = false;
            }
        }

        if (disableAnimatorsImmediately)
        {
            for (int i = 0; i < animators.Length; i++)
            {
                if (animators[i] != null)
                    animators[i].enabled = false;
            }
        }

        if (disableBehavioursImmediately)
        {
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] != null && behaviours[i] != this)
                {
                    behaviours[i].enabled = false;
                }
            }
        }

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < despawnFlickerDuration)
        {
            elapsed += Random.Range(despawnFlickerMinInterval, despawnFlickerMaxInterval);

            visible = !visible;
            SetRenderersVisible(spriteRenderers, visible);

            yield return new WaitForSeconds(Random.Range(despawnFlickerMinInterval, despawnFlickerMaxInterval));
        }

        SetRenderersVisible(spriteRenderers, false);
        target.SetActive(false);
    }

    private void SetRenderersVisible(SpriteRenderer[] renderers, bool visible)
    {
        if (renderers == null || renderers.Length == 0)
            return;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].enabled = visible;
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
                _SpawnedPrompt.GetComponentsInChildren<Text>()[2].text = PromptWhenOn();
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