using UnityEngine;

[CreateAssetMenu(fileName = "New Grid Recipe", menuName = "Crafting/GridRecipe")]
public class GridRecipe : ScriptableObject
{
    public string recipeName;
    public Item result;
    public int resultAmount = 1;
    public Sprite icon;

    [Tooltip("Lista de 9 ítems (null = vacío), ordenados de izquierda a derecha")]
    public Item[] pattern = new Item[9];
}
