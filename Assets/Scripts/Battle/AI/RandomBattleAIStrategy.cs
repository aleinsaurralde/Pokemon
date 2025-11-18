using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomAIStrategy : IBattleAIStrategy
{
    public Move ChooseMove(Pokemon self, Pokemon target)
    {
        var movesWithPP = self.Moves.Where(x => x.PP > 0).ToList();

        if (movesWithPP.Count == 0)
            return self.Moves[0];

        return movesWithPP[Random.Range(0, movesWithPP.Count)];
    }
}