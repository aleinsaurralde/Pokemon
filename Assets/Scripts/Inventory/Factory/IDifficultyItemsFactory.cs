using System.Collections.Generic;

public interface IDifficultyItemsFactory
{
    List<ItemSlot> GetStartingItems(ItemFactory itemFactory);
}

[System.Serializable]
public class StartingItemConfig
{
    public string itemName;
    public int quantity;
}