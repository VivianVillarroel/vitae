using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public List<Item> items = new List<Item>();
    public Action OnItemChangedCallback;
    public int space = 20; // Espacio máximo

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Más de una instancia de Inventory encontrada!");
            return;
        }
        instance = this;
    }

    // Añadir un ítem al inventario
    public bool Add(Item item)
    {
        // Verifica si ya existe el ítem
        foreach (Item i in items)
        {
            if (i.itemName == item.itemName && i.maxStack > 1)
            {
                i.currentAmount++;
                OnItemChangedCallback?.Invoke();
                return true;
            }
        }

        if (items.Count >= space)
        {
            Debug.Log("Inventario lleno.");
            return false;
        }

        // 🔧 Clonar el ítem antes de agregarlo para evitar modificar el original ScriptableObject
        Item newItem = Instantiate(item);
        newItem.currentAmount = 1;

        items.Add(newItem);
        OnItemChangedCallback?.Invoke();
        return true;
    }


    // Remover un ítem
    public void Remove(Item item)
    {
        items.Remove(item);

        OnItemChangedCallback?.Invoke();
    }
}