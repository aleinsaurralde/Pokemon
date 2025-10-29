using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PartyScreenState : IGameState
{
    private BattleSystem _battleSystem;
    public PartyScreenState(BattleSystem battleSystem)
    {
        _battleSystem = battleSystem;
    }

    public void Enter()
    {
        _battleSystem.PartyScreen.SetPartyData(_battleSystem.playerParty.Pokemons);
        _battleSystem.PartyScreen.gameObject.SetActive(true);
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            _battleSystem.currentMember++;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            _battleSystem.currentMember--;

        _battleSystem.currentMember = Mathf.Clamp(_battleSystem.currentMember, 0, _battleSystem.playerParty.Pokemons.Count - 1);

        _battleSystem.PartyScreen.UpdateMemberSelection(_battleSystem.currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = _battleSystem.playerParty.Pokemons[_battleSystem.currentMember];
            if (selectedMember.HP <= 0)
            {
                _battleSystem.PartyScreen.SetMessageText("You can't send out a fainted pokemon!");
                return;
            }
            if (selectedMember == _battleSystem.PlayerUnit.Pokemon)
            {
                _battleSystem.PartyScreen.SetMessageText($"{selectedMember} is already on the field!");
                return;
            }

            _battleSystem.PartyScreen.gameObject.SetActive(false);

            if (_battleSystem.prevState == _battleSystem.states[typeof(ActionSelectionState)])
            {
                _battleSystem.prevState = null;
                _battleSystem.StartCoroutine(_battleSystem.RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                _battleSystem.ChangeState<BusyState>();
                _battleSystem.StartCoroutine(_battleSystem.SwitchPokemon(selectedMember));
            }

        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (_battleSystem.PlayerUnit.Pokemon.HP > 0)
            {
                _battleSystem.PartyScreen.gameObject.SetActive(false);
                _battleSystem.ActionSelection();
            }
            else
            {
                _battleSystem.PartyScreen.SetMessageText("You have to send out a pokemon!");
                return;
            }
        }
    }
    public void Exit()
    {
        _battleSystem.PartyScreen.gameObject.SetActive(false);

    }
}
