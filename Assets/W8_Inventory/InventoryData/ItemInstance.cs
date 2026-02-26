using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    public ItemData itemType;
    public Sprite sprite; //we can assign the ItemData defaults in our constructor method
    public string description; //we can assign the ItemData defaults in our constructor method
    public int qty; //this data is really only used for the Inventory Array example.

    //let's say you want to be able to update/edit
    //the item data for different instances of the same item

    //you can make a constructor method that
    //takes the ItemData as a default template,
    //but you can eventually pass different information
    //into this instance's variables later on.

    public ItemInstance(ItemData itemData)
    {
        itemType = itemData;
        sprite = itemData.icon;
        description = itemData.description;
        qty = 0;
    }

    //you might customise this constructor
    //for generating different versions of this item
    //(e.g. pickup items with different qtys)

    public ItemInstance(ItemData itemData, int itemQty)
    {
        itemType = itemData;
        sprite = itemData.icon;
        description = itemData.description;
        qty = itemQty;
    }
}