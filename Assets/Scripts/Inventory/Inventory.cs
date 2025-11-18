using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<ItemSlot> slots;

    public List<ItemSlot> Slots => slots;

    public ItemBase UseItem(int indexItem, Pokemon selectedPokemon)
    {
        var item = slots[indexItem].Item;
        bool itemUsed = item.Use(selectedPokemon);

        if (itemUsed)
        {
            RemoveItem(item);
            return item;
        }

        return null;
    }

    public void InitializeWithDifficulty()
    {
        Initialize();
        var startingItems = DifficultyManager.Instance.GetStartingItems();

        foreach (var itemSlot in startingItems)
        {
            AddItem(itemSlot.Item, itemSlot.Count);
        }

        Debug.Log($"Inventory initialized with {startingItems.Count} item types");
    }
    public void Initialize()
    {
        if (slots == null)
            slots = new List<ItemSlot>();
        else
            slots.Clear();
    }

    public void AddItems(List<ItemBase> items)
    {
        foreach (var item in items)
        {
            AddItem(item, 1);
        }
    }

    private void RemoveItem(ItemBase item)
    {
        var itemSlot = slots.First(slot => slot.Item == item);
        itemSlot.Count--;
        if (itemSlot.Count == 0)
        {
            slots.Remove(itemSlot);
        }
    }

    public void AddItem(ItemBase item, int count = 1)
    {
        var existingSlot = slots.FirstOrDefault(slot => slot.Item != null && slot.Item.Name == item.Name);

        if (existingSlot != null)
        {
            existingSlot.Count += count;
        }
        else
        {
            var newSlot = new ItemSlot(item, count);
            slots.Add(newSlot);
        }
        Debug.Log($"Added {count} {item.Name} to inventory");
    }


}

[Serializable]
public class ItemSlot
{
    [SerializeField] private ItemBase item;
    [SerializeField] private int count;

    public ItemBase Item => item;
    public int Count
    {
        get => count;
        set => count = value;
    }

    public ItemSlot(ItemBase item, int count)
    {
        this.item = item;
        this.count = count;
    }
}
