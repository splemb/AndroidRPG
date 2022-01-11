using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    public Inventory backpack;
    [SerializeField] Transform weaponPivot;

    private void Start()
    {
        backpack.ResetInventory();
    }

    private void OnTriggerStay(Collider other)
    {
        if (GetComponent<TouchMovement>().playState == TouchMovement.PlayState.Moving)
        {
            if (other.CompareTag("Item"))
            {
                if (backpack.AddItem(other.GetComponent<ItemContainer>().itemObject, other.GetComponent<ItemContainer>().quantity))
                    Destroy(other.gameObject);
            }
        }
    }

    public void DropSlot(ItemSlot itemSlot)
    {
        backpack.DropItem(itemSlot);
    }

    public void UseItem(ItemSlot itemSlot)
    {
        backpack.UseItem(itemSlot);
        if (itemSlot.itemObject.itemType == ItemObject.Types.Weapon) ChangeWeaponModel(itemSlot.itemObject.heldItemPrefab);
    }

    public void ChangeWeaponModel(GameObject newModel)
    {
        foreach (Transform child in weaponPivot)
        {
            GameObject.Destroy(child.gameObject);
        }

        if (newModel != null) Instantiate(newModel, weaponPivot);
    }
}
