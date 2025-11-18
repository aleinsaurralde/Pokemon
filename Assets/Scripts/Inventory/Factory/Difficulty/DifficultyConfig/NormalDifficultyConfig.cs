using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/NormalDifficultyConfig")]
public class NormalDifficultyConfig : ScriptableObject, IDifficultyItemsFactory
{
    [Header("Normal Difficulty Items")]
    [SerializeField] private int superPotionCount = 1;

    public List<ItemSlot> GetStartingItems(ItemFactory itemFactory)
    {
        var items = new List<ItemSlot>();

        // Super Potion
        var superPotion = itemFactory.GetHealthItem("Super Potion");
        if (superPotion != null)
            items.Add(new ItemSlot(superPotion, superPotionCount));

        return items;
    }
}