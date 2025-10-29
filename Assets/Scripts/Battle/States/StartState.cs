using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class StartState : IGameState
{
    private readonly BattleSystem battleSystem;

    public StartState(BattleSystem system)
    {
        battleSystem = system;
    }

    public void Enter()
    {
        battleSystem.StartCoroutine(Setup());
    }

    private IEnumerator Setup()
    {
        battleSystem.PlayerUnit.Setup(battleSystem.playerParty.GetHealthyPokemon());
        battleSystem.EnemyUnit.Setup(battleSystem.wildPokemon);
        battleSystem.PartyScreen.Init();
        battleSystem.DialogBox.SetMoveNames(battleSystem.PlayerUnit.Pokemon.Moves);

        yield return battleSystem.DialogBox.TypeDialog($"A wild {battleSystem.EnemyUnit.Pokemon.Base.Name} appeared!");
        battleSystem.ChangeState<ActionSelectionState>();
    }

    public void HandleUpdate() { }
    public void Exit() { }
}
