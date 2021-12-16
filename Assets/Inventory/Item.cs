using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemObject", menuName = "Item System/Add Item", order = 2)]
public class ItemObject : ScriptableObject
{
    public GameObject itemPrefab; // dropped item model
    public Sprite icon; // inventory sprite

    public string itemName;
    [TextArea(4, 16)]
    public string description;

    public float weight = 0f; // optional for weight system
    public int maxStackableQuantity = 1; // used for stacks of items

    public bool useOnPickup = true; // if true, used on pick up
    public bool consumable = true; // if true, single use item
}