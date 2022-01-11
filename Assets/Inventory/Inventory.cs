using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventory", menuName = "Item System/Add Inventory", order = 1)]
public class Inventory : ScriptableObject
{
    public int maxSlots;
    public List<ItemSlot> itemSlots = new List<ItemSlot>();
    public ItemSlot weapon;
    public ItemSlot armour;

    public bool AddItem(ItemObject itemObject, int quantity)
    {
        //Returns true if the item was successfully added to the inventory

        

        //Instant consume health pickup
        if (itemObject.itemType == ItemObject.Types.HealthPickup)
        {
            UseItem(itemObject);
            return true;
        }

        //Check for space in existing item stacks
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].itemObject == itemObject && itemSlots[i].stackQuantity < itemObject.maxStackableQuantity)
            {
                quantity = itemSlots[i].AddToStack(quantity);
                if (quantity <= 0)
                {
                    GameObject.Find("Sounds").GetComponent<SoundPlayer>().PlaySound(2, 0.3f);
                    return true;
                }
            }
        }

        //If any items are left, make a new stack
        if (quantity > 0)
        {
            if (itemSlots.Count < maxSlots)
            {
                itemSlots.Add(new ItemSlot(itemObject, quantity));
                GameObject.Find("Sounds").GetComponent<SoundPlayer>().PlaySound(2, 0.3f);
                return true;
            }
        }

        return false;
    }

    public void DropItem(ItemSlot itemSlot)
    {
        //Removes a given item slot from the inventory

        ItemObject item = itemSlot.itemObject;
        if (item.itemPrefab != null)
        {
            Transform playerTransform = GameObject.FindGameObjectWithTag("PlayerModel").transform;

            //Instantiates the dropped item in pickup form in front of the player, with slight variation on position
            for (int i = 0; i < itemSlot.stackQuantity; i++)
            {
                Vector3 dropPosition = playerTransform.position;
                Vector3 offset = playerTransform.right * Random.Range(-2f, 2f) + playerTransform.forward * Random.Range(0f,1f);
                dropPosition += offset + playerTransform.forward;
                GameObject droppedItem = Instantiate(item.itemPrefab, dropPosition, Quaternion.identity);
                droppedItem.GetComponent<Rigidbody>().AddForce(playerTransform.forward * 0.1f + Vector3.up);
            }
        }
        itemSlots.Remove(itemSlot);
    }

    //Unequips weapon and re-adds to inventory
    public void UnequipWeapon()
    {
        if (weapon.itemObject == null) return;
        AddItem(weapon.itemObject, 1);
        weapon.itemObject = null;
    }

    public void UnequpArmour()
    {
        if (armour.itemObject == null) return;
        AddItem(armour.itemObject, 1);
        armour.itemObject = null;
    }

    //Uses the item in an item slot
    public void UseItem(ItemSlot itemSlot)
    {
        switch (itemSlot.itemObject.itemType)
        {
            case ItemObject.Types.Weapon:
                if (weapon.itemObject != null)
                {
                    if (!AddItem(weapon.itemObject, 1)) DropItem(weapon);
                }
                weapon.itemObject = itemSlot.itemObject;
                itemSlots.Remove(itemSlot);
                break;
            case ItemObject.Types.Armour:
                if (armour.itemObject != null)
                {
                    if (!AddItem(armour.itemObject, 1)) DropItem(armour);
                }
                armour.itemObject = itemSlot.itemObject;
                itemSlots.Remove(itemSlot);
                break;
            case ItemObject.Types.HealthPotion:
                GameObject.FindGameObjectWithTag("Player").GetComponent<TouchMovement>().Heal(1000f);
                itemSlot.RemoveFromStack(1);
                break;
        }
    }

    //Uses an item passed in directly (used for health pickups)
    public void UseItem(ItemObject itemObject)
    {
        if (itemObject.itemType == ItemObject.Types.HealthPickup)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<TouchMovement>().Heal(10f);
        }
    }

    //Built-in sort algroithm using the name of the item in the item slot as the sort parameter
    public void SortInventory()
    {
        itemSlots.Sort(SortByName);
    }

    static int SortByName(ItemSlot one, ItemSlot two)
    {
        return one.itemObject.itemName.CompareTo(two.itemObject.itemName);
    }

    //Clears the inventory
    public void ResetInventory()
    {
        itemSlots = new List<ItemSlot>();
        weapon.itemObject = null;
        armour.itemObject = null;
    }
}

[System.Serializable]
public class ItemSlot
{
    public ItemObject itemObject; // actual item
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

    public int RemoveFromStack(int quantity)
    {
        stackQuantity -= quantity;

        return stackQuantity;
    }
}

