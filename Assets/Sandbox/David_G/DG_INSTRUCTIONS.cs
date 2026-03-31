using System;
using System.Collections.Generic;
using UnityEngine;

public class DG_INSTRUCTIONS : MonoBehaviour
{


    [SerializeField] private string axisName;

    void Start()
    {
        if (axisName == null)
        {
            Debug.LogError("No UI instruction axisName, please set the axisName in the inspector.");
        }
    }

    void Update()
    {
        if (Input.GetButtonDown(axisName))
        {
            Destroy(gameObject);
        }
    }
}
