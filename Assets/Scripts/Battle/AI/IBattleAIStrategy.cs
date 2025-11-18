using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleAIStrategy
{
    Move ChooseMove(Pokemon self, Pokemon target);
}
