using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/HardDifficultyConfig")]
public class HardDifficultyConfig : ScriptableObject, IDifficultyItemsFactory
{
    [Header("Hard Difficulty Items")]
    [SerializeField] private int potionCount = 1;

    public List<ItemSlot> GetStartingItems(ItemFactory itemFactory)
    {
        var items = new List<ItemSlot>();

        // Potion
        var potion = itemFactory.GetHealthItem("Potion");
        if (potion != null)
            items.Add(new ItemSlot(potion, potionCount));
        return items;
    }
}