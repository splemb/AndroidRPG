using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryRenderer : MonoBehaviour
{
    [SerializeField] Inventory backpack; //given inventory
    [SerializeField] Transform content; //ui element where item slots are displayed
    [SerializeField] GameObject infoBox; //info box displays further information on item

    [SerializeField] GameObject prefabItemSlot; //prefab of the item slot instantiated
    [SerializeField] GameObject weaponItemSlot;
    [SerializeField] GameObject armourItemSlot;

    [SerializeField] AudioClip[] audioClips;
    [SerializeField] AudioSource audioSource;

    private void OnEnable()
    {
        RenderInventory(); //setup inventory when it is reopened
        GameObject.Find("Sounds").GetComponent<SoundPlayer>().PlaySound(0);
    }

    public void RenderInventory()
    {
        ClearInventory();
        backpack.SortInventory();
        foreach (ItemSlot slot in backpack.itemSlots)
        {
            GameObject newItemSlot = Instantiate(prefabItemSlot, content);
            newItemSlot.GetComponent<ItemSlotInfo>().Setup(slot);
        }

        //dummy empty slots are used when no weapon or armour is equipped
        if (backpack.weapon.itemObject != null) weaponItemSlot.GetComponent<ItemSlotInfo>().Setup(backpack.weapon);
        else weaponItemSlot.GetComponent<ItemSlotInfo>().DummySlot();

        if (backpack.armour.itemObject != null) armourItemSlot.GetComponent<ItemSlotInfo>().Setup(backpack.armour);
        else armourItemSlot.GetComponent<ItemSlotInfo>().DummySlot();
    }

    void ClearInventory()
    {
        //Clears item slots ready to be reinstantiated
        foreach (Transform child in content)
        {
            GameObject.Destroy(child.gameObject);
        }

    }

    //Refreshes inventory and removes item model from player
    public void UnequipWeapon()
    {
        backpack.UnequipWeapon();
        GameObject.FindGameObjectWithTag("Player").GetComponent<ItemInteraction>().ChangeWeaponModel(null);
        RenderInventory();
    }

    //Refreshes inventory
    public void UnequipArmour()
    {
        backpack.UnequpArmour();
        RenderInventory();
    }

    // Opens/closes info box when item slot pressed
    
    public void OpenInfoBox(ItemSlot itemSlot)
    {
        infoBox.SetActive(true);
        infoBox.GetComponent<InfoBox>().UpdateInformation(itemSlot);
        GameObject.Find("Sounds").GetComponent<SoundPlayer>().PlaySound(1);
    }

    public void CloseInfoBox()
    {
        infoBox.SetActive(false);
        GameObject.Find("Sounds").GetComponent<SoundPlayer>().PlaySound(1);
    }
}
