using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    public CraftingStation station;
    public GameObject buttonPrefab;
    public Transform buttonParent;

    void Start()
    {
        foreach (var recipe in station.availableRecipes)
        {
            GameObject btn = Instantiate(buttonPrefab, buttonParent);
            btn.GetComponentInChildren<Text>().text = recipe.recipeName;
            btn.GetComponent<Button>().onClick.AddListener(() => {
                station.TryCraft(recipe);
            });
        }
    }
}
