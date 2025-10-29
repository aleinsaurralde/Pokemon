using UnityEngine;

public class MoveSelectionState : IGameState
{
    private readonly BattleSystem bs;

    public MoveSelectionState(BattleSystem system)
    {
        bs = system;
    }

    public void Enter()
    {
        bs.DialogBox.EnableActionSelector(false);
        bs.DialogBox.EnableDialogText(false);
        bs.DialogBox.EnableMoveSelector(true);
        bs.currentMove = 0;
        bs.DialogBox.UpdateMoveSelection(bs.currentMove, bs.PlayerUnit.Pokemon.Moves[bs.currentMove]);
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) ++bs.currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) --bs.currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) bs.currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow)) bs.currentMove -= 2;

        bs.currentMove = Mathf.Clamp(bs.currentMove, 0, bs.PlayerUnit.Pokemon.Moves.Count - 1);
        bs.DialogBox.UpdateMoveSelection(bs.currentMove, bs.PlayerUnit.Pokemon.Moves[bs.currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var move = bs.PlayerUnit.Pokemon.Moves[bs.currentMove];
            if (move.PP <= 0) return;

            bs.DialogBox.EnableMoveSelector(false);
            bs.DialogBox.EnableDialogText(true);
            bs.StartCoroutine(bs.RunTurns(BattleAction.Move));
            bs.ChangeState<RunningTurnState>();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            bs.DialogBox.EnableMoveSelector(false);
            bs.DialogBox.EnableDialogText(true);
            bs.ChangeState<ActionSelectionState>();
        }
    }

    public void Exit() { }
}
