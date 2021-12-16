using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventory", menuName = "Item System/Add Inventory", order = 1)]
public class Inventory : ScriptableObject
{
    public List<ItemSlot> itemSlots = new List<ItemSlot>();

    public void AddItem(ItemObject itemObject, int quantity)
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].itemObject == itemObject && itemSlots[i].stackQuantity < itemObject.maxStackableQuantity)
            {
                quantity = itemSlots[i].AddToStack(quantity);
                if (quantity <= 0)
                {
                    break;
                }
            }
        }

        if (quantity > 0)
        {
            itemSlots.Add(new ItemSlot(itemObject, quantity));
        }
    }
}

[System.Serializable]
public class ItemSlot
{
    public ItemObject itemObject;
    public int stackQuantity; // how many you have

    public ItemSlot(ItemObject itemObject, int stackQuantity)
    {
        this.itemObject = itemObject;
        this.stackQuantity = stackQuantity;
    }

    public int AddToStack(int quantity)
    {
        stackQuantity += quantity;
        int remainingQuantity = stackQuantity - itemObject.maxStackableQuantity;

        if (remainingQuantity < 0)
        {
            remainingQuantity = 0;
        }

        stackQuantity -= remainingQuantity;

        return remainingQuantity;
    }
}

