using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public List<Item> items = new List<Item>();
    public Action OnItemChangedCallback;
    public int space = 20; // Espacio máximo

    public Item selectedItem;
    public Transform handPoint; // Asignar en el inspector (es la "mano" del jugador)
    private GameObject currentHandItem;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Más de una instancia de Inventory encontrada!");
            return;
        }
        instance = this;

        // Asignar automáticamente handPoint si está en escena y no se asignó manualmente
        if (handPoint == null)
        {
            GameObject found = GameObject.Find("HandPoint");
            if (found != null)
            {
                handPoint = found.transform;
                Debug.Log("✅ handPoint asignado automáticamente desde escena: " + handPoint.name);
            }
        }
    }

    void Start()
    {
        if (handPoint == null)
        {
            Debug.LogError("🚫 [Inventory.cs] handPoint es NULL. Asigna manualmente en el inspector o asegúrate que el objeto se llame 'HandPoint'.");
        }
        else
        {
            Debug.Log("✅ [Inventory.cs] handPoint ASIGNADO correctamente: " + handPoint.name);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseSelectedItem();
        }
    }

    void UseSelectedItem()
    {
        if (selectedItem == null) return;

        if (selectedItem.itemType == Item.ItemType.Consumible)
        {
            Debug.Log("Consumido: " + selectedItem.itemName);

            PlayerHealth playerHealth = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.isDead)
            {
                playerHealth.currentHealth = Mathf.Min(playerHealth.currentHealth + 30, playerHealth.maxHealth);
                playerHealth.SendMessage("UpdateHealthUI", SendMessageOptions.DontRequireReceiver);
                Debug.Log("✅ Se restauraron 30 puntos de vida");
            }

            selectedItem.currentAmount--;
            if (selectedItem.currentAmount <= 0)
            {
                items.Remove(selectedItem);
                if (currentHandItem != null) Destroy(currentHandItem);
            }

            OnItemChangedCallback?.Invoke();
        }
        else if (selectedItem.itemType == Item.ItemType.Construible)
        {
            Debug.Log("🏠 Modo construcción activado: " + selectedItem.itemName);

            // Aquí podrías hacer que el BuilderSystem espere el click para colocar la construcción
            BuilderSystem builder = GameObject.FindObjectOfType<BuilderSystem>();
            if (builder != null)
            {
                builder.PrepareToBuild(selectedItem);
            }
        }
        else
        {
            Debug.Log("No se puede usar directamente este ítem.");
        }
    }



    // Añadir un ítem al inventario
    public bool Add(Item item)
    {
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

        Item newItem = Instantiate(item);
        newItem.currentAmount = 1;

        items.Add(newItem);
        OnItemChangedCallback?.Invoke();
        return true;
    }

    public void SelectItem(Item item)
    {
        selectedItem = item;
        Debug.Log("✅ Seleccionado: " + item.itemName);

        if (currentHandItem != null)
        {
            Destroy(currentHandItem);
        }

        if (handPoint == null || item.worldPrefab == null)
        {
            Debug.LogWarning("⚠️ Faltan referencias: handPoint o worldPrefab es null.");
            return;
        }

        currentHandItem = Instantiate(item.worldPrefab, handPoint.position, handPoint.rotation, handPoint);

        // 🔇 Eliminar comportamiento tipo mundo
        var floatScript = currentHandItem.GetComponent<FloatingItem>();
        if (floatScript != null) Destroy(floatScript);

        // 🔇 Desactivar colisionadores (opcional)
        foreach (var col in currentHandItem.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        // 🧱 Escala dinámica según tipo de ítem
        switch (item.itemType)
        {
            case Item.ItemType.Arma:
                currentHandItem.transform.localScale = Vector3.one * 1.0f; // tamaño real
                break;
            case Item.ItemType.Herramienta:
                currentHandItem.transform.localScale = Vector3.one * 0.7f;
                break;
            case Item.ItemType.Consumible:
            case Item.ItemType.Material:
                currentHandItem.transform.localScale = Vector3.one * 0.2f;
                break;
            default:
                currentHandItem.transform.localScale = Vector3.one * 0.5f;
                break;
        }

        Debug.Log($"✅ Instanciado en mano con escala: {currentHandItem.transform.localScale}");
    }




    public void Remove(Item item)
    {
        items.Remove(item);
        OnItemChangedCallback?.Invoke();
    }
}
