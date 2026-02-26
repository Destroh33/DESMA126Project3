using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class InventoryListManager : ScriptableObject
{
    public List<ItemInstance> inventory = new(); // contains item instances
    public int maxCapacity = 6; // maximum inventory slots available

    public bool HasSpaceForNewItem(ItemInstance itemToAdd)
    {

        //do we have an empty slot in our list?
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == null)
            {
                //if so, let's assign this slot to the new item!
                inventory[i] = itemToAdd;
                return true;
            }
        }

        //is the size of our inventory list less than max capacity?
        if (inventory.Count < maxCapacity)
        {
            //if so, let's make a new slot, and add our new item there!
            inventory.Add(itemToAdd);
            return true;
        }

        //otherwise, we don't have space for a new item!
        Debug.Log("No space in inventory.");
        return false;
    }

}

