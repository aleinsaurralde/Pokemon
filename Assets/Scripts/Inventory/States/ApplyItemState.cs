using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyItemState : IGameState
{
    private BattleSystem _battleSystem;
    private BagSelectionState _bagSelectionState;
    public ApplyItemState(BattleSystem battleSystem, BagSelectionState bagSelectionState)
    {
        _battleSystem = battleSystem;
        _bagSelectionState = bagSelectionState;
    }

    public void Enter()
    {
        _battleSystem.ChangeState<PartyScreenState>();
    }
    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            _bagSelectionState.ChangeState<ItemSelectionState>();
        }
    }
    public void Exit()
    {

    }
}
