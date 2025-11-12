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
        if (!battleSystem.IsTrainerBattle)
        {
            //wild pkmn
            battleSystem.PlayerUnit.Setup(battleSystem.playerParty.GetHealthyPokemon());
            battleSystem.EnemyUnit.Setup(battleSystem.wildPokemon);
            battleSystem.DialogBox.SetMoveNames(battleSystem.PlayerUnit.Pokemon.Moves);

            yield return battleSystem.DialogBox.TypeDialog($"A wild {battleSystem.EnemyUnit.Pokemon.Base.Name} appeared!");
        }
        else
        {
            //trainer
            battleSystem.PlayerUnit.gameObject.SetActive(false);
            battleSystem.EnemyUnit.gameObject.SetActive(false);

            battleSystem.PlayerImage.gameObject.SetActive(true);
            battleSystem.TrainerImage.gameObject.SetActive(true);

            battleSystem.PlayerImage.sprite = battleSystem.Player.GetComponentInChildren<Sprite>();
            battleSystem.TrainerImage.sprite = battleSystem.Trainer.GetComponentInChildren<Sprite>();

            yield return battleSystem.DialogBox.TypeDialog($"{battleSystem.Trainer.name} wants to battle!");
        }
        
        battleSystem.PartyScreen.Init();
        
        battleSystem.ChangeState<ActionSelectionState>();
    }

    public void HandleUpdate() { }
    public void Exit() { }
}
