using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class InventoryArrayManager : ScriptableObject
{
    public ItemInstance[] inventory; 
        // contains item instances

    public ItemData[] allPossibleItems;
        // the player either has or doesn't have the item
        // so inventory size remains fixed

        // (but visibility of item UI graphics get toggled off
        // if item qty is 0 -- see: InventoryArrayDisplay.cs)

    //initialise array, if not yet already done so.
    public void ResetInventory()
    {
        if (inventory.Length == 0)
        {
            inventory = new ItemInstance[allPossibleItems.Length];
            for (int i =0; i < inventory.Length; i++)
            {
                inventory[i] = new ItemInstance(allPossibleItems[i]);
            }
        }
    }

    //checks if we are removing the last item
    //we will use this to determine whether UI graphics for that item
    //should be switched off.
    public bool IsRemovingLastItem(ItemInstance item)
    {
        item.qty--;

        if (item.qty == 0)
        {
            return true;
        } else
        {
            return false;
        }
    }

}



