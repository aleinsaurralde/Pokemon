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


        if (Input.GetKeyDown(KeyCode.F5)) 
        {
            SavingSystem.i.Save("save");
        }
        if (Input.GetKeyDown(KeyCode.F6)) 
        {
            SavingSystem.i.Load("save");
        }
    }

    public void Exit()
    {
        // Nada por ahora, pero podría pausar movimiento, etc.
    }
}
