using System.Linq;

public class SmartAIStrategy : IBattleAIStrategy
{
    public Move ChooseMove(Pokemon self, Pokemon opponent)
    {
        var moves = self.Moves.Where(m => m.PP > 0).ToList();
        if (moves.Count == 0)
            return self.Moves[0];

        Move bestMove = moves[0];
        float bestScore = -9999f;

        foreach (var move in moves)
        {
            float score = EvaluateMove(move, self, opponent);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }

    private float EvaluateMove(Move move, Pokemon self, Pokemon opponent)
    {
        var moveBase = move.Base;

        // 1) Moves sin power → ataques de estado
        if (moveBase.Power <= 0)
            return EvaluateStatusMove(move, self, opponent);

        // 2) Base: daño bruto según estadísticas
        float attackStat = moveBase.Category == MoveCategory.Special ? self.SpAttack : self.Attack;
        float defenseStat = moveBase.Category == MoveCategory.Special ? opponent.SpDefense : opponent.Defense;

        float levelFactor = (2f * self.Level / 5f) + 2f;
        float baseDamage = ((levelFactor * moveBase.Power * (attackStat / defenseStat)) / 50f) + 2;

        // 3) STAB
        float stab = (moveBase.Type == self.Base.Type1 || moveBase.Type == self.Base.Type2) ? 1.5f : 1f;

        // 4) Type Effectiveness
        float effectiveness = TypeChart.GetEffectiveness(moveBase.Type, opponent.Base.Type1);
        if (opponent.Base.Type2 != PokemonType.None)
            effectiveness *= TypeChart.GetEffectiveness(moveBase.Type, opponent.Base.Type2);

        float estimatedDamage = baseDamage * stab * effectiveness;

        // 5) Bonus si puede matar en este turno
        if (estimatedDamage >= opponent.HP)
            estimatedDamage *= 2f;

        // 6) Penalización si es poco efectivo
        if (effectiveness < 1f)
            estimatedDamage *= 0.7f;

        // 7) Gran penalización si es inmune
        if (effectiveness == 0f)
            estimatedDamage = -999f;

        return estimatedDamage;
    }

    private float EvaluateStatusMove(Move move, Pokemon self, Pokemon opponent)
    {
        var moveBase = move.Base;

        // Moves sin power tienen score mucho más bajo que un ataque efectivo
        float score = 10f;

        // Si el enemigo tiene mucha vida → conviene aplicar estados
        if (opponent.HP > opponent.MaxHp * 0.5f)
            score += 20f;

        // Bonus por probabilidad de aplicar estado
        if (moveBase.Effects != null && moveBase.Effects.Status != ConditionID.none)
            score += 30f;

        return score;
    }
}
