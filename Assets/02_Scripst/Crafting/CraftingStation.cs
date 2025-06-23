using UnityEngine;
using System.Collections.Generic;

public class CraftingStation : MonoBehaviour
{
    public List<CraftingRecipe> availableRecipes;

    public void TryCraft(CraftingRecipe recipe)
    {
        if (CanCraft(recipe))
        {
            foreach (var ingredient in recipe.ingredients)
            {
                RemoveItemsFromInventory(ingredient.item, ingredient.amount);
            }

            for (int i = 0; i < recipe.resultAmount; i++)
            {
                Inventory.instance.Add(recipe.resultItem);
            }

            Debug.Log($"✅ ¡Has creado {recipe.resultItem.itemName}!");
        }
        else
        {
            Debug.Log("🚫 No tienes los materiales suficientes.");
        }
    }

    public bool CanCraft(CraftingRecipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            int count = CountItemInInventory(ingredient.item);
            if (count < ingredient.amount)
            {
                return false;
            }
        }
        return true;
    }

    int CountItemInInventory(Item item)
    {
        int total = 0;
        foreach (var i in Inventory.instance.items)
        {
            if (i.itemName == item.itemName)
                total += i.currentAmount;
        }
        return total;
    }

    void RemoveItemsFromInventory(Item item, int amount)
    {
        int remaining = amount;
        List<Item> itemsToRemove = new List<Item>();

        foreach (var i in Inventory.instance.items)
        {
            if (i.itemName == item.itemName)
            {
                int take = Mathf.Min(i.currentAmount, remaining);
                i.currentAmount -= take;
                remaining -= take;

                if (i.currentAmount <= 0)
                    itemsToRemove.Add(i);

                if (remaining <= 0)
                    break;
            }
        }

        foreach (var i in itemsToRemove)
        {
            Inventory.instance.Remove(i);
        }

        Inventory.instance.OnItemChangedCallback?.Invoke();
    }
}
