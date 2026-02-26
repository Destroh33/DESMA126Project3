using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    [TextArea] public string description;
    //The "TextArea" attribute enables a height-flexible
    //and scrollable text input field inside our inspector.
}

//this example only uses a single type of item data.
//depending on your project needs,
//you may consider making multiple types of item data
//according to more specialised classes
//(e.g. equipable items, consumable items)