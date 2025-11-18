using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveBattleAIStrategy : IBattleAIStrategy
{
    public Move ChooseMove(Pokemon self, Pokemon opponent)
    {
        Move best = null;
        int bestPower = -1;

        foreach (var move in self.Moves)
        {
            if (move.PP <= 0)
                continue;

            int power = move.Base.Power;  // daño base

            if (power > bestPower)
            {
                bestPower = power;
                best = move;
            }
        }

        return best ?? self.Moves[0];
    }
}
