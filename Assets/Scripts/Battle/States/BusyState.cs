using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusyState : IGameState
{
    private BattleSystem _battleSystem;
    public BusyState(BattleSystem battleSystem)
    {
        _battleSystem = battleSystem;
    }

    public void Enter()
    {

    }

    public void HandleUpdate()
    {

    }

    public void Exit()
    {

    }
}
