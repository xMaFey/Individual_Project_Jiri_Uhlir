using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Inventory Inventory;

    //Use this for initialization
    void Start()
    {
        //Inventory.ItemAdded += InventoryScript_ItemAdded;
    }

    private void InventoryScript_ItemAdded(object sender)
    {
        Transform inventoryPanel = transform.Find("InventoryPanel");
        foreach (Transform slot in inventoryPanel)
        {
            //Border... Image
            Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();

            //We found the empty slot
            if (!image.enabled)
            {
                image.enabled = true;
                //image.sprite = e.Item.Image;

                break;
            }
        }
    }
}
