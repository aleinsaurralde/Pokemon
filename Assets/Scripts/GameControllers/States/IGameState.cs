public interface IGameState
{
    void Enter();
    void HandleUpdate();
    void Exit();
}