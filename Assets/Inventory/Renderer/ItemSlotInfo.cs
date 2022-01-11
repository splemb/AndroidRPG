using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotInfo : MonoBehaviour
{
    [SerializeField] ItemSlot itemSlot;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI quantityText;
    [SerializeField] Image image;

    //Displays item info in the item slot
    public void Setup(ItemSlot newItemSlot)
    {
        itemSlot = newItemSlot;
        nameText.enabled = true;
        image.enabled = true;
        nameText.text = newItemSlot.itemObject.itemName;
        image.sprite = newItemSlot.itemObject.icon;

        //Quantity
        if (itemSlot != null && quantityText != null)
        {
            if (itemSlot.stackQuantity > 1 && quantityText)
                quantityText.text = "x" + itemSlot.stackQuantity.ToString();
            else
                quantityText.text = " ";
        }
    }

    public void DummySlot()
    {
        image.enabled = false;
        nameText.enabled = false;
        quantityText.enabled = false;
    }

    //Function that responds to pressing item slot
    public void InfoBox()
    {
        GameObject.Find("InventoryScreen").GetComponent<InventoryRenderer>().OpenInfoBox(itemSlot);
    }
}
