using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class CritCalculator
{
    public static long CalculateHitRewardUnits(int basePowerUnits, long score, CritConfigSO config, out bool isCrit)
    {
        if (config == null)
        {
            isCrit = false;
            return basePowerUnits;
        }

        var chance = config.CalculateChance(score);
        var roll = Random.value;
        isCrit = roll <= chance;
        if (!isCrit) return basePowerUnits;

        var range = config.CalculateMultiplierRange(score);
        var coeff = Random.Range(range.x, range.y);
        return Convert.ToInt64(Math.Max(UIntValue.UnitsPerCoin, (double)basePowerUnits * coeff));
    }
}
