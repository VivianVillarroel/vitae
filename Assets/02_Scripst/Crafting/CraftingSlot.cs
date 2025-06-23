using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingSlot : MonoBehaviour, IPointerClickHandler
{
    public int index;
    public CraftingGridUI craftingUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        Item selected = Inventory.instance.selectedItem;
        if (selected != null)
        {
            craftingUI.SetItemInSlot(index, selected);
        }
    }
}
