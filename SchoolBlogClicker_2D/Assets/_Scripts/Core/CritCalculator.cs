using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class CritCalculator
{
    public static long CalculateHitReward(int basePower, long score, CritConfigSO config, out bool isCrit)
    {
        if (config == null)
        {
            isCrit = false;
            return basePower;
        }

        var chance = config.CalculateChance(score);
        var roll = Random.value;
        isCrit = roll <= chance;
        if (!isCrit) return basePower;

        var range = config.CalculateMultiplierRange(score);
        var coeff = Random.Range(range.x, range.y);
        return Convert.ToInt64(Math.Max(1f, basePower * coeff));
    }
}
