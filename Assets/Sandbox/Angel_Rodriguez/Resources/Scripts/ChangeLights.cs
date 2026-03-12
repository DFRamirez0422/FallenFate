using UnityEngine;
using UnityEngine.Rendering.Universal;


public class ChangeLights : MonoBehaviour
{
    [SerializeField] private Powered_Door powered_Door;

    [Header("Door Lights Sprites")]
    [SerializeField] private Sprite Door_Light_Left_On;
    [SerializeField] private Light2D Door_left_Light_Color;
    [SerializeField] private Sprite Door_Light_Right_On;
    [SerializeField] private Light2D Door_Right_Light_Color;
    [SerializeField] private Sprite Door_Light_On;


    void Start()
    {
        Door_left_Light_Color.color = new Color(254f, 0f, 0f); // Set left light color to red
        Door_Right_Light_Color.color = new Color(254f, 0f, 0f); // Set right light color to red
    }

    void Update()
    {
        Color on = new Color(0f, 254f, 0f);           //(130f, 254f, 49f);

        if (powered_Door.activateGenerators == null || powered_Door.activate_Generator2 == null)
        {
            Debug.LogError("Activate_Generators references are not set in the inspector.");
            return;
        }

        if (powered_Door.activateGenerators.Activate_Generator && !powered_Door.activate_Generator2.Activate_Generator)
        {
            this.GetComponent<SpriteRenderer>().sprite = Door_Light_Left_On; // Change sprite to left light on
            Door_left_Light_Color.color = on; // Change left light color to green
        }
        else if (!powered_Door.activateGenerators.Activate_Generator && powered_Door.activate_Generator2.Activate_Generator)
        {
            this.GetComponent<SpriteRenderer>().sprite = Door_Light_Right_On; // Change sprite to right light on
            Door_Right_Light_Color.color = on; // Change right light color to green
        }
        else if (powered_Door.activateGenerators.Activate_Generator && powered_Door.activate_Generator2.Activate_Generator)
        {
            this.GetComponent<SpriteRenderer>().sprite = Door_Light_On; // Change sprite to both lights on
            Door_left_Light_Color.color = on; // Change left light color to green
            Door_Right_Light_Color.color = on; // Change right light color to green
        }
    }


}