using UnityEngine;

public class CraftingStationClickable : MonoBehaviour
{
    public GameObject craftingPanel; // Asigna el panel de crafteo desde el inspector

    private void OnMouseDown()
    {
        float distance = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);

        if (distance <= 3f && craftingPanel != null)
        {
            craftingPanel.SetActive(true);
            TogglePlayerControl(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
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
}
