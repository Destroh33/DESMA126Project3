using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryListDisplay : MonoBehaviour
{
    public InventoryListManager inventoryManager;

    public ItemData[] allPossibleItems; // this is only for the random item button instantation

    public GameObject[] inventoryItems; // inventory slots; each represents 1 slot.

    //for UI graphics
    public GameObject itemInfoPanelGroup;
    public Image itemIcon;
    public TextMeshProUGUI tmp_itemName, tmp_itemDesc;
    public int currentSelectedItem;

    //for calling the spawn item function when item is used.
    public ItemSpawner itemSpawner;

    public void Awake()
    {
        UpdateList(); //update our inventory at the beginning of the scene.
    }

    public void UpdateItemInfoPanel(int index)
    {
        currentSelectedItem = index;
        //if item info panel group is inactive
        if (!itemInfoPanelGroup.activeSelf)
        {
            //set it to active
            itemInfoPanelGroup.SetActive(true);
        }

        itemIcon.sprite = inventoryManager.inventory[index].sprite;
        tmp_itemName.text = inventoryManager.inventory[index].itemType.itemName;
        tmp_itemDesc.text = inventoryManager.inventory[index].description;
    }

    //update inventory-related graphics to match
    //most recent inventory data.
    public void UpdateList()
    {
        for (int i =0; i<inventoryItems.Length; i++)
        {
            //is our current inventory slot length less than the inventory list count?
            if (i < inventoryManager.inventory.Count)
            {
                if (inventoryManager.inventory[i] != null)
                {
                    if (!inventoryItems[i].activeSelf)
                    {
                        inventoryItems[i].SetActive(true);
                    }
                    inventoryItems[i].GetComponent<Image>().sprite = inventoryManager.inventory[i].sprite;
                }
                else
                {
                    if (inventoryItems[i].activeSelf)
                    {
                        inventoryItems[i].SetActive(false);
                    }
                }
            }
            else
            {
                if (inventoryItems[i].activeSelf)
                {
                    inventoryItems[i].SetActive(false);
                }
            }
        }
    }


    //------methods for button functionality---------

    //you can modify the following according to your project needs:

    //---ADDING ITEMS INTO THE INVENTORY
    public void AddRandomItem()
    {
        ItemInstance randomItem = new ItemInstance(allPossibleItems[Random.Range(0, allPossibleItems.Length)]);

        if (inventoryManager.HasSpaceForNewItem(randomItem)) //returns true if we have available inventory slots.
        {
            UpdateList();
        }
    }

    //---USING / DROPPING ITEMS FROM THE INVENTORY
    public void UseItem()
    {
        itemSpawner.SpawnNewItem(inventoryManager.inventory[currentSelectedItem].sprite);

        inventoryManager.inventory.Remove(inventoryManager.inventory[currentSelectedItem]);
        UpdateList();
        itemInfoPanelGroup.SetActive(false);
    }
}
