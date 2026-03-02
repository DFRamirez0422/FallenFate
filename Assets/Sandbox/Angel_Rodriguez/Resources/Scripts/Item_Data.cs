using UnityEngine;

// Define a ScriptableObject to hold item data
[CreateAssetMenu(fileName = "Item_Data", menuName = "ScriptableObjects/Item_Data", order = 1)]
public class Item_Data : ScriptableObject
{
   public enum PickUpType
    {   
        Memento,
        keys,
        other
    }
    public PickUpType pickupType;
    public string itemName;
    public GameObject item;    
    public string UniqueID;
}
