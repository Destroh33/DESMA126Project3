using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryArrayDisplay : MonoBehaviour
{
    public InventoryArrayManager inventoryManager;
    public GameObject[] inventoryItems;

    public GameObject itemInfoPanelGroup;
    public Image itemIcon;
    public TextMeshProUGUI tmp_itemName, tmp_itemDesc;


    int currentSelectedItem;
    public ItemSpawner itemSpawner;


    public void Awake()
    {
        inventoryManager.ResetInventory();
        UpdateList();
    }

    public void AddRandomItem()
    {
        inventoryManager.inventory[Random.Range(0, inventoryManager.inventory.Length)].qty++;
        UpdateList();
    }

    public void UseItem()
    {
        itemSpawner.SpawnNewItem(inventoryManager.inventory[currentSelectedItem].sprite);

        if (inventoryManager.IsRemovingLastItem(inventoryManager.inventory[currentSelectedItem]))
        {
            inventoryItems[currentSelectedItem].SetActive(false);
        }

        UpdateList();
        itemInfoPanelGroup.SetActive(false);
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
    
    public void UpdateList()
    {
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryManager.inventory[i].qty>0)
            {
                if (!inventoryItems[i].activeSelf)
                {
                    inventoryItems[i].SetActive(true);
                }
                inventoryItems[i].GetComponent<Image>().sprite = inventoryManager.inventory[i].sprite;
                inventoryItems[i].transform.GetComponentInChildren<TextMeshProUGUI>().text = inventoryManager.inventory[i].qty.ToString();
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


}
