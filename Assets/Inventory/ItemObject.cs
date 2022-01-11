using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemObject", menuName = "Item System/Add Item", order = 2)]
public class ItemObject : ScriptableObject
{
    public enum Types { HealthPotion, Armour, Weapon, Treasure, HealthPickup, FireAmmo }
    [SerializeField] public Types itemType;

    public GameObject itemPrefab; // dropped item model
    public GameObject heldItemPrefab; //held item model
    public GameObject projectilePrefab; //fired item model 
    public ItemObject ammo; //the item object used as ammo
    public Sprite icon; // inventory sprite

    public float atkBonus; //attack value provided
    public float defBonus; //defense value provided

    public string itemName;
    [TextArea(4, 16)]
    public string description;

    public int maxStackableQuantity = 1; // used for stacks of items
}