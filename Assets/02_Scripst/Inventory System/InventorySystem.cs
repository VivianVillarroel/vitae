// InventorySystem.cs
using UnityEngine;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    public int inventorySize = 24;
    public int hotbarSize = 9;
    public List<InventorySlot> slots = new List<InventorySlot>();

    [System.Serializable]
    public class InventorySlot
    {
        public Item item;
        public int amount;
    }

    void Start()
    {
        InitializeInventory();
    }

    void InitializeInventory()
    {
        slots = new List<InventorySlot>();
        for (int i = 0; i < inventorySize; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    public bool AddItem(Item item, int amount)
    {
        // Primero intenta apilar en slots existentes
        foreach (InventorySlot slot in slots)
        {
            if (slot.item != null && slot.item.id == item.id && slot.amount < item.maxStack)
            {
                int spaceRemaining = item.maxStack - slot.amount;
                int amountToAdd = Mathf.Min(spaceRemaining, amount);
                slot.amount += amountToAdd;
                amount -= amountToAdd;

                if (amount <= 0) return true;
            }
        }

        // Luego busca slots vacíos
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == null)
            {
                slot.item = item;
                slot.amount = Mathf.Min(amount, item.maxStack);
                amount -= slot.amount;

                if (amount <= 0) return true;
            }
        }

        return false; // Inventario lleno
    }
}