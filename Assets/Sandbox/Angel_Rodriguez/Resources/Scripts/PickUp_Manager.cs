using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

//Updated to manage picked up items using a list of Item_Data ScriptableObjects
// Manages the list of picked up items
public class PickUp_Manager : MonoBehaviour
{
    public List<Item_Data> items;
}
