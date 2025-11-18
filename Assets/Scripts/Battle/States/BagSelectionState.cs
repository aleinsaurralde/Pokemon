using System;
using System.Collections.Generic;
using UnityEngine;

public class BagSelectionState : IGameState
{
    private BattleSystem _battleSystem;
    private Inventory _inventory;
    public Dictionary<Type, IGameState> states { get; private set; }
    private IGameState currentState;

    private int selectedItem = 0;
    public BagSelectionState(BattleSystem battleSystem, Inventory inventory)
    {
        _battleSystem = battleSystem;
        _inventory = inventory;

        states = new Dictionary<Type, IGameState>
        {
            { typeof(ItemSelectionState), new ItemSelectionState(battleSystem, inventory, this) },
            { typeof(ApplyItemState), new ApplyItemState(battleSystem, this) },
        };
    }



    public void Enter()
    {
        _battleSystem.InventoryUI.UpdateItemList();
        ChangeState<ItemSelectionState>();

    }

    public void HandleUpdate()
    {
        currentState?.HandleUpdate();
    }

    

    public void Exit()
    {
        _battleSystem.InventoryUI.gameObject.SetActive(false);
    }

    public void ChangeState<T>() where T : IGameState
    {
        currentState?.Exit();
        currentState = states[typeof(T)];
        currentState.Enter();
    }
}
