using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon = null;
    public int maxStack = 64;
    public ItemType itemType;

    [HideInInspector]
    public int currentAmount = 1; // por defecto, 1 unidad


    // 🔧 Agrega esta línea:
    public GameObject worldPrefab;

    public enum ItemType
    {
        Consumible,
        Material,
        Arma,
        Herramienta
    }

    public virtual void Use()
    {
        Debug.Log("Usando: " + itemName);
    }
}
