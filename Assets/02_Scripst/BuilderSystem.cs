using UnityEngine;

public class BuilderSystem : MonoBehaviour
{
    [Header("Configuración")]
    public LayerMask groundLayer;
    public float rayDistance = 100f;
    public Material hologramMaterial;

    private Item itemToBuild;
    private GameObject previewObject;
    public float yOffset = 0.5f;

    private float currentRotation = 0f;

    void Update()
    {
        if (itemToBuild != null)
        {
            UpdatePreview();

            if (Input.GetKeyDown(KeyCode.E))
            {
                PlaceObject();
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelBuild();
            }

            // Rotar holograma
            if (Input.GetKeyDown(KeyCode.Q)) currentRotation -= 15f;
            if (Input.GetKeyDown(KeyCode.R)) currentRotation += 15f;
        }
    }

    public void PrepareToBuild(Item item)
    {
        itemToBuild = item;

        if (itemToBuild.worldPrefab == null)
        {
            Debug.LogError("❌ El ítem no tiene prefab asignado.");
            itemToBuild = null;
            return;
        }

        previewObject = Instantiate(itemToBuild.worldPrefab);
        SetPreviewMode(previewObject);
        Debug.Log("👁️ Modo construcción con holograma activado.");
    }

    void UpdatePreview()
    {
        Camera cam = Camera.main;
        Vector3 origin = cam.transform.position;
        Vector3 direction = cam.transform.forward;

        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(origin, direction * rayDistance, Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, groundLayer))
        {
            Vector3 pos = hit.point + Vector3.up * yOffset;
            previewObject.transform.position = pos;
            previewObject.transform.rotation = Quaternion.Euler(0, currentRotation, 0);

            Debug.Log($"✅ Construcción apuntando al terreno en: {hit.point}");
        }
        else
        {
            Debug.LogWarning("🚫 No se detectó terreno.");
        }
    }

    void PlaceObject()
    {
        if (previewObject == null || itemToBuild == null) return;

        Vector3 pos = previewObject.transform.position;
        Quaternion rot = previewObject.transform.rotation;

        Instantiate(itemToBuild.worldPrefab, pos, rot);

        itemToBuild.currentAmount--;
        if (itemToBuild.currentAmount <= 0)
        {
            Inventory.instance.Remove(itemToBuild);
        }

        // 🧼 Limpia ítem de la mano y la selección
        Inventory.instance.selectedItem = null;

        if (Inventory.instance.handPoint.childCount > 0)
        {
            foreach (Transform child in Inventory.instance.handPoint)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        Inventory.instance.OnItemChangedCallback?.Invoke();
        Debug.Log("✅ Objeto construido.");

        CancelBuild();
    }


    void CancelBuild()
    {
        if (previewObject != null)
            Destroy(previewObject);

        itemToBuild = null;
        previewObject = null;
        currentRotation = 0f;
        Debug.Log("🚫 Construcción cancelada.");
    }

    void SetPreviewMode(GameObject obj)
    {
        obj.transform.localScale *= 0.9f;

        foreach (var col in obj.GetComponentsInChildren<Collider>())
            col.enabled = false;

        foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
            renderer.material = hologramMaterial;
    }
}
