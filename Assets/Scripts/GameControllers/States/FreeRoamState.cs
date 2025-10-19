using UnityEngine;

public class FreeRoamState : IGameState
{
    private GameController gameController;
    private PlayerController playerController;

    public FreeRoamState(GameController gameController, PlayerController playerController)
    {
        this.gameController = gameController;
        this.playerController = playerController;
    }

    public void Enter()
    {
        // Se ejecuta cuando entramos al modo libre
        gameController.BattleSystem.gameObject.SetActive(false);
        gameController.WorldCamera.gameObject.SetActive(true);
    }

    public void HandleUpdate()
    {
        playerController.HandleUpdate();
    }

    public void Exit()
    {
        // Nada por ahora, pero podría pausar movimiento, etc.
    }
}
