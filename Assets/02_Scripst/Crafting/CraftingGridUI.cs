using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingGridUI : MonoBehaviour
{
    [Header("Slots y UI")]
    public Image[] craftingSlotsUI; // 9 imágenes del grid
    public Item[] craftingGrid = new Item[9];
    public Image resultSlotUI;
    public Button craftButton;


    [Header("Recetas y Panel Izquierdo")]
    public Transform recetasContainer;
    public GameObject recetaButtonPrefab;

    private List<GridRecipe> allRecipes = new List<GridRecipe>();
    private Item resultItem;
    private int resultAmount;

    void Start()
    {
        craftButton.onClick.AddListener(Craft);
        LoadRecipes();
    }

    void LoadRecipes()
    {
        allRecipes.AddRange(Resources.LoadAll<GridRecipe>("Recipes"));

        foreach (var receta in allRecipes)
        {
            GameObject btn = Instantiate(recetaButtonPrefab, recetasContainer);
            btn.GetComponentInChildren<Text>().text = receta.recipeName;
            btn.GetComponentInChildren<Image>().sprite = receta.icon;
            btn.GetComponent<Button>().onClick.AddListener(() => AutoFillGrid(receta));
        }
    }

    void AutoFillGrid(GridRecipe receta)
    {
        for (int i = 0; i < 9; i++)
        {
            craftingGrid[i] = receta.pattern[i];
        }
        UpdateCraftingUI();
    }

    public void UpdateCraftingUI()
    {
        for (int i = 0; i < 9; i++)
        {
            if (craftingGrid[i] != null)
            {
                craftingSlotsUI[i].sprite = craftingGrid[i].icon;
                craftingSlotsUI[i].enabled = true;
            }
            else
            {
                craftingSlotsUI[i].sprite = null;
                craftingSlotsUI[i].enabled = false;
            }
        }


        resultItem = null;
        resultAmount = 0;
        resultSlotUI.enabled = false;

        foreach (var receta in allRecipes)
        {
            if (Match(receta.pattern))
            {
                resultItem = receta.result;
                resultAmount = receta.resultAmount;
                resultSlotUI.sprite = resultItem.icon;
                resultSlotUI.enabled = true;
                break;
            }
        }
    }
    public void ClosePanel()
    {
        gameObject.SetActive(false);
        TogglePlayerControl(true);

        // ✅ En lugar de Lock, simplemente activa el cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void TogglePlayerControl(bool enabled)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.GetComponent<MovementPlayer>().enabled = enabled;
            player.GetComponent<PlayerCombat>().enabled = enabled;
        }
    }
    bool Match(Item[] pattern)
    {
        for (int i = 0; i < 9; i++)
        {
            if (craftingGrid[i] != pattern[i]) return false;
        }
        return true;
    }

    public void Craft()
    {
        if (resultItem == null)
        {
            Debug.Log("🚫 No hay receta válida.");
            return;
        }

        for (int i = 0; i < 9; i++)
        {
            if (craftingGrid[i] == null) continue;
            if (!HasItem(craftingGrid[i])) { Debug.Log("🚫 Faltan materiales."); return; }
        }

        for (int i = 0; i < 9; i++)
        {
            if (craftingGrid[i] != null)
            {
                RemoveOne(craftingGrid[i]);
                craftingGrid[i] = null;
            }
        }

        Inventory.instance.Add(resultItem);
        UpdateCraftingUI();
    }

    bool HasItem(Item item)
    {
        foreach (var i in Inventory.instance.items)
        {
            if (i.itemName == item.itemName && i.currentAmount > 0) return true;
        }
        return false;
    }

    void RemoveOne(Item item)
    {
        foreach (var i in Inventory.instance.items)
        {
            if (i.itemName == item.itemName)
            {
                i.currentAmount--;
                if (i.currentAmount <= 0) Inventory.instance.Remove(i);
                break;
            }
        }
        Inventory.instance.OnItemChangedCallback?.Invoke();
    }

    public void SetItemInSlot(int index, Item item)
    {
        craftingGrid[index] = item;
        UpdateCraftingUI();
    }
}
