using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlot;

    [SerializeField] private Inventory inventory;

    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemDescription;

    public Image ItemIcon => itemIcon;
    public TextMeshProUGUI ItemDescription => itemDescription;

    private List<ItemSlotUI> slotUIList;

    public List<ItemSlotUI> SlotUIList => slotUIList;

    public void UpdateItemList()
    {
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var item in inventory.Slots)
        {
            var newItem = Instantiate(itemSlot);
            newItem.transform.SetParent(itemList.transform);
            newItem.SetData(item);
            slotUIList.Add(newItem);
        }
    }
}
