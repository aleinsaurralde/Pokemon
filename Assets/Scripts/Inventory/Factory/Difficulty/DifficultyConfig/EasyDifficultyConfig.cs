using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/EasyDifficultyConfig")]
public class EasyDifficultyConfig : ScriptableObject, IDifficultyItemsFactory
{
    [Header("Easy Difficulty Items")]
    [SerializeField] private int hyperPotionCount = 2;

    public List<ItemSlot> GetStartingItems(ItemFactory itemFactory)
    {
        var items = new List<ItemSlot>();

        // Hyper Potions
        var hyperPotion = itemFactory.GetHealthItem("Hyper Potion");
        if (hyperPotion != null)
            items.Add(new ItemSlot(hyperPotion, hyperPotionCount));

        return items;
    }
}