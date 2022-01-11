using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoBox : MonoBehaviour
{
    [SerializeField] ItemSlot itemSlot;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI quantityText;
    [SerializeField] Image image;

    [SerializeField] Button useButton;
    [SerializeField] Inventory backpack;

    public void UpdateInformation(ItemSlot newItemSlot)
    {
        

        itemSlot = newItemSlot;
        nameText.text = newItemSlot.itemObject.itemName;
        descriptionText.text = newItemSlot.itemObject.description;
        image.sprite = newItemSlot.itemObject.icon;
        if (itemSlot.stackQuantity > 1)
            quantityText.text = "Quantity: " + itemSlot.stackQuantity.ToString();
        else
            quantityText.text = " ";

        useButton.gameObject.SetActive(true);
        switch (itemSlot.itemObject.itemType)
        {
            case (ItemObject.Types.Weapon):
                useButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
                break;
            case (ItemObject.Types.Armour):
                useButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
                break;
            case (ItemObject.Types.HealthPotion):
                useButton.GetComponentInChildren<TextMeshProUGUI>().text = "Drink";
                break;
            default:
                useButton.gameObject.SetActive(false);
                break;
        }
    }

    public void DropSlot()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInteraction>().DropSlot(itemSlot);
        GameObject.Find("InventoryScreen").GetComponent<InventoryRenderer>().CloseInfoBox();
        GameObject.Find("InventoryScreen").GetComponent<InventoryRenderer>().RenderInventory();
    }

    public void UseItem()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInteraction>().UseItem(itemSlot);
        GameObject.Find("InventoryScreen").GetComponent<InventoryRenderer>().CloseInfoBox();
        GameObject.Find("InventoryScreen").GetComponent<InventoryRenderer>().RenderInventory();
    }

}
