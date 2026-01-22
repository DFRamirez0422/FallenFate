using UnityEngine;

[CreateAssetMenu(fileName = "Item_Data", menuName = "ScriptableObjects/Item_Data", order = 1)]
public class Item_Data : ScriptableObject
{
   public enum PickUpType
    {   
        Memento,
        keys
    }
    public PickUpType pickupType;
    public string itemName = "New Item";
    public GameObject item;    
    public bool collected;
}
