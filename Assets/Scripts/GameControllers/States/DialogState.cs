using System;

public class DialogState : IGameState
{

    public void Enter()
    {

    }

    public void Exit()
    {

    }

    public void HandleUpdate()
    {
        DialogManager.Instance.HandleUpdate();
    }

}
