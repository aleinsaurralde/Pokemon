public class RunningTurnState : IGameState
{
    private readonly BattleSystem bs;

    public RunningTurnState(BattleSystem system)
    {
        bs = system;
    }

    public void Enter()
    {
        // Lógica de turnos ya corre desde la corutina en BattleSystem
    }

    public void HandleUpdate() { }
    public void Exit() { }
}
