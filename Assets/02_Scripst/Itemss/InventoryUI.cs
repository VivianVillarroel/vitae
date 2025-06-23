using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public GameObject inventoryUI;
    Inventory inventory;
    InventorySlot[] slots;

    private int selectedIndex = 0;
    public Image selectionHighlight; // UI para resaltar el slot seleccionado

    void Start()
    {
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();

        inventory = Inventory.instance;
        inventory.OnItemChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
        UpdateUI(); // Mostrar inicialmente
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }

        for (int i = 0; i < Mathf.Min(slots.Length, 9); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectSlot(i);
            }
        }
    }

    void SelectSlot(int index)
    {
        if (inventory == null || inventory.items == null)
        {
            Debug.LogError("Inventory no está inicializado.");
            return;
        }

        if (index >= inventory.items.Count)
        {
            Debug.LogWarning("No hay ítem en el slot " + index);
            return;
        }

        selectedIndex = index;

        Item selectedItem = inventory.items[selectedIndex];

        if (selectedItem == null)
        {
            Debug.LogWarning("El ítem en el índice " + index + " es null.");
            return;
        }

        inventory.SelectItem(selectedItem);

        if (selectionHighlight != null && slots != null && slots[selectedIndex] != null)
        {
            selectionHighlight.transform.position = slots[selectedIndex].transform.position;
        }
        else
        {
            Debug.LogWarning("selectionHighlight o el slot seleccionado es null.");
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}