using System.Collections.Generic;
using UnityEngine;

public class ItemSelectionState : IGameState
{
    private BattleSystem _battleSystem;
    private Inventory _inventory;
    private BagSelectionState _bagSelectionState;

    private int selectedItem = 0;
    public ItemSelectionState(BattleSystem battleSystem, Inventory inventory, BagSelectionState bagSelectionState)
    {
        _battleSystem = battleSystem;
        _inventory = inventory;
        _bagSelectionState = bagSelectionState;
    }
    public void Enter()
    {
        _battleSystem.InventoryUI.gameObject.SetActive(true);
        UpdateItemSelection();
    }

    public void HandleUpdate()
    {
        int prevItem = selectedItem;

        if (Input.GetKeyDown(KeyCode.DownArrow)) selectedItem++;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) selectedItem--;

        selectedItem = Mathf.Clamp(selectedItem, 0, _inventory.Slots.Count - 1);

        if (prevItem != selectedItem)
        {
            UpdateItemSelection();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            _battleSystem.usingItem = true;
            _bagSelectionState.ChangeState<ApplyItemState>();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            _battleSystem.ChangeState<ActionSelectionState>();
        }
    }

    private void UpdateItemSelection()
    {
        for (int i = 0; i < _battleSystem.InventoryUI.SlotUIList.Count; i++)
        {
            if (i == selectedItem)
            {
                _battleSystem.InventoryUI.SlotUIList[i].NameText.color = Color.blue;
            }
            else
            {
                _battleSystem.InventoryUI.SlotUIList[i].NameText.color = Color.black;
            }

            var slot = _inventory.Slots[selectedItem];

            _battleSystem.InventoryUI.ItemIcon.sprite = slot.Item.Icon;
            _battleSystem.InventoryUI.ItemDescription.text = slot.Item.Description;
            _battleSystem.itemIndex = selectedItem;
        }
    }
    public void Exit()
    {
        _battleSystem.InventoryUI.gameObject.SetActive(false);
    }
}
