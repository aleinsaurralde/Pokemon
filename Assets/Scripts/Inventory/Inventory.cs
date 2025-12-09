using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour, ISaveable
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
    }

    public object CaptureState()
    {
        var saveData = new BagSaveData
        {
            slots = slots.Select(s => s.GetSaveData()).ToList(),
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (BagSaveData)state;

        slots = saveData.slots.Select(s => new ItemSlot(s)).ToList();
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

    public ItemSlotSaveData GetSaveData()
    {
        return new ItemSlotSaveData
        {
            itemName = item.Name,
            count = count,
        };
    }
    public ItemSlot(ItemSlotSaveData saveData)
    {
        item = ItemDB.GetItemByName(saveData.itemName);
        count = saveData.count;
    }
}
[Serializable]
public class BagSaveData
{
    public List<ItemSlotSaveData> slots;
}

[Serializable]
public class ItemSlotSaveData
{
    public string itemName;
    public int count;
} 
