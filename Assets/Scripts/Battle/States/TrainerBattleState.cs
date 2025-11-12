using UnityEngine;

public class TrainerBattleState : IGameState
{
    private GameController gameController;
    private BattleSystem battleSystem;
    private PlayerController playerController;
    private TrainerController trainerController;

    public TrainerBattleState(GameController gameController, BattleSystem battleSystem, PlayerController playerController, TrainerController trainerController)
    {
        this.gameController = gameController;
        this.battleSystem = battleSystem;
        this.playerController = playerController;
        this.trainerController = trainerController;
    }

    public void Enter()
    {
        battleSystem.gameObject.SetActive(true);
        gameController.WorldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var trainerParty = trainerController.GetComponent<PokemonParty>();

        battleSystem.StartTrainerBattle(playerParty, trainerParty);
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
