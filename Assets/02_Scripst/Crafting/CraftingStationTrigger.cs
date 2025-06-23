using UnityEngine;

public class CraftingStationTrigger : MonoBehaviour
{
    public GameObject craftingPanel; // Asignar PanelCrafting desde el inspector
    public float interactDistance = 3f;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (craftingPanel != null)
        {
            craftingPanel.SetActive(false); // Ocultar al inicio
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= interactDistance && Input.GetKeyDown(KeyCode.E))
        {
            if (craftingPanel != null)
            {
                craftingPanel.SetActive(true);
                Time.timeScale = 0f; // Pausa si deseas
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
