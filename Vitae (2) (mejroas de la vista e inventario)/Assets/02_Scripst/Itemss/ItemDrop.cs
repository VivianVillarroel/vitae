using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public Item[] possibleDrops;
    public float dropChance = 0.5f; // 50% de probabilidad

    public void DropItem()
    {
        Debug.Log("[DropItem] Intentando dropear ítem...");

        if (possibleDrops == null || possibleDrops.Length == 0)
        {
            Debug.LogWarning("[DropItem] No hay ítems asignados en PossibleDrops.");
            return;
        }

        float roll = Random.value;
        Debug.Log($"[DropItem] Probabilidad generada: {roll} / Chance requerida: {dropChance}");

        if (roll <= dropChance)
        {
            Item itemToDrop = possibleDrops[Random.Range(0, possibleDrops.Length)];
            if (itemToDrop == null)
            {
                Debug.LogError("[DropItem] El ítem seleccionado es NULL.");
                return;
            }

            Debug.Log($"[DropItem] Dropeando ítem: {itemToDrop.itemName}");

            if (itemToDrop.worldPrefab != null)
            {
                Instantiate(itemToDrop.worldPrefab, transform.position + Vector3.up, Quaternion.identity);
                Debug.Log($"[DropItem] Ítem instanciado en el mundo: {itemToDrop.itemName}");
            }
            else
            {
                Debug.LogWarning($"[DropItem] El ítem '{itemToDrop.itemName}' no tiene prefab asignado.");
            }
        }
        else
        {
            Debug.Log("[DropItem] Falló la tirada. No se dropeará ningún ítem.");
        }
    }
}
