using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Text countText;
    Item item;

    public void AddItem(Item newitem)
    {
        item = newitem;
        icon.sprite = item.icon;
        icon.enabled = true;
        countText.text = item.currentAmount > 1 ? item.currentAmount.ToString() : "";
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        countText.text = "";
    }

    public void OnSlotClicked()
    {
        if (item != null)
        {
            Inventory.instance.SelectItem(item);
        }
    }
}