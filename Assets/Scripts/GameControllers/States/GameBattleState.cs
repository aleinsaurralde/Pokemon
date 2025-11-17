using UnityEngine;

public class GameBattleState : IGameState
{
    private GameController gameController;
    private BattleSystem battleSystem;
    private PlayerController playerController;

    public GameBattleState(GameController gameController, BattleSystem battleSystem, PlayerController playerController)
    {
        this.gameController = gameController;
        this.battleSystem = battleSystem;
        this.playerController = playerController;
    }

    public void Enter()
    {
        battleSystem.gameObject.SetActive(true);
        gameController.WorldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = Object.FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();

        var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);

        battleSystem.StartWildBattle(playerParty, wildPokemon);
    }

    public void HandleUpdate()
    {
        battleSystem.HandleUpdate();
    }

    public void Exit()
    {
        battleSystem.gameObject.SetActive(false);
        gameController.WorldCamera.gameObject.SetActive(true);
    }
}
