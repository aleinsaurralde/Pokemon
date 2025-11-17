using UnityEngine;

public class ActionSelectionState : IGameState
{
    private readonly BattleSystem bs;

    public ActionSelectionState(BattleSystem system)
    {
        bs = system;
    }

    public void Enter()
    {
        bs.DialogBox.SetDialog("Choose an action");
        bs.DialogBox.EnableActionSelector(true);
        bs.currentAction = 0;
        bs.DialogBox.UpdateActionSelection(bs.currentAction);
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) ++bs.currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) --bs.currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) bs.currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) bs.currentAction -= 2;

        bs.currentAction = Mathf.Clamp(bs.currentAction, 0, 3);
        bs.DialogBox.UpdateActionSelection(bs.currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            switch (bs.currentAction)
            {
                case 0:
                    bs.ChangeState<MoveSelectionState>();
                    break;
                case 1:
                    // Item state futuro
                    bs.StartCoroutine(bs.RunTurns(BattleAction.UseItem));
                    break;
                case 2:
                    bs.ChangeState<PartyScreenState>();
                    break;
                case 3:
                    // Run state futuro
                    bs.StartCoroutine(bs.RunTurns(BattleAction.Run));
                    break;
            }
        }
    }

    public void Exit()
    {
        bs.DialogBox.EnableActionSelector(false);
    }
}
