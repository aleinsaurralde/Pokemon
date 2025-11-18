using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleAIStrategyFactory
{
    public static IBattleAIStrategy CreateStrategy(EnemyAIStrategyType type)
    {
        return type switch
        {
            EnemyAIStrategyType.Random => new RandomAIStrategy(),
            EnemyAIStrategyType.Aggressive => new AggressiveBattleAIStrategy(),
            EnemyAIStrategyType.Smart => new SmartAIStrategy(),
            _ => new RandomAIStrategy()
        };
    }
}
