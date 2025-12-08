using UnityEngine;

public static class BattleFormulas
{
    public static bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.Base.AlwaysHit) return true;

        float moveAccuracy = move.Base.Accuracy;
        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];

        return Random.Range(1, 101) <= moveAccuracy;
    }

    public static string GetEffectivenessMessage(float typeEffectiveness)
    {
        return typeEffectiveness switch
        {
            > 1f => "It's super effective!",
            < 1f and > 0f => "It's not very effective...",
            0f => "It had no effect...",
            _ => null
        };
    }

    public static bool CanEscape(int playerSpeed, int enemySpeed, int attempts)
    {
        if (enemySpeed < playerSpeed) return true;

        float f = (playerSpeed * 128) / enemySpeed + 30 * attempts;
        f = f % 256;

        return Random.Range(0, 255) < f;
    }

    public static int CalculateExperienceGain(Pokemon faintedPokemon, bool isTrainerBattle)
    {
        int expYield = faintedPokemon.Base.ExpYield;
        int enemyLevel = faintedPokemon.Level;
        float trainerBonus = isTrainerBattle ? 1.5f : 1f;

        // Fórmula simplificada de experiencia
        return Mathf.FloorToInt(expYield * enemyLevel * trainerBonus) / 7;
    }
}